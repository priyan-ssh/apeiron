#!/usr/bin/env node

import { Command } from 'commander';
import prompts from 'prompts';
import fs from 'fs-extra';
import path from 'path';
import picocolors from 'picocolors';
import { fileURLToPath } from 'url';
import figlet from 'figlet';
import gradient from 'gradient-string';


const { cyan, green, red, bold, yellow } = picocolors;
const __dirname = path.dirname(fileURLToPath(import.meta.url));

const program = new Command();

const displayBanner = () => {
    const banner = figlet.textSync('APEIRON', { font: 'Doom' });
    const coloredBanner = gradient('red', 'orange', 'gold').multiline(banner);
    console.log(coloredBanner);
    console.log(bold(red('  THE FORGE\n')));
};

program
    .name('apeiron')
    .description('Scaffold a new Production-Ready APEIRON project')
    .version('1.0.0')
    .argument('[project-directory]', 'Directory to scaffold the project in')
    .action(async (targetDir) => {
        const enterAltScreen = () => process.stdout.write('\x1b[?1049h');
        const exitAltScreen = () => process.stdout.write('\x1b[?1049l');

        let header = ""; // Store header to reprint on exit if needed

        const cleanup = (code = 0) => {
            exitAltScreen();
            process.exit(code);
        };

        process.on('SIGINT', () => {
            exitAltScreen();
            console.log(red('✖ Operation aborted by user.'));
            process.exit(0);
        });

        enterAltScreen();
        displayBanner();

        let projectDir = targetDir;
        let features = [];

        // Wizard Mode
        if (!projectDir) {
            const response = await prompts([
                {
                    type: 'text',
                    name: 'dir',
                    message: 'Project Name?',
                    initial: 'my-apeiron-app'
                },
                {
                    type: 'multiselect',
                    name: 'features',
                    message: 'Select Industrial Modules:',
                    choices: [
                        { title: 'Authentication (JWT)', value: 'auth', selected: true },
                        { title: 'Hybrid Caching (Redis)', value: 'cache', selected: true },
                        { title: 'OpenTelemetry', value: 'otel', selected: true },
                        { title: 'Docker Compose', value: 'docker', selected: true }
                    ]
                }
            ]);

            if (!response.dir) {
                exitAltScreen(); // Exit Alt Screen BEFORE printing final message
                console.log(red('✖ Operation aborted.'));
                process.exit(1);
            }
            projectDir = response.dir;
            features = response.features;
        }

        const fullPath = path.resolve(process.cwd(), projectDir);
        const templatesDir = path.resolve(__dirname, '../templates');

        // Check/Clean Directory
        if (fs.existsSync(fullPath)) {
            const { overwrite } = await prompts({
                type: 'confirm',
                name: 'overwrite',
                message: `Target directory ${projectDir} exists. Overwrite?`,
                initial: false
            });

            if (!overwrite) {
                exitAltScreen();
                console.log(red('✖ Protocol halted.'));
                process.exit(1);
            }
            await fs.emptyDir(fullPath);
        } else {
            await fs.ensureDir(fullPath);
        }

        // --- SCAFFOLDING PHASE (Switch back to Main Screen for logs?) ---
        // Actually, user wants "Back to previous state", implying logs should be in Alt Screen?
        // OR logs persist? "with necessary history and messages"
        // Let's keep Scaffolding in Alt Screen, but print FINAL summary after exit.

        console.log(cyan(`\n🔧 Scaffolding infrastructure in ${bold(projectDir)}...`));

        // Scaffold (Copy Templates)
        if (!fs.existsSync(templatesDir)) {
            exitAltScreen();
            console.log(red(`✖ Critical Failure: Templates not found at ${templatesDir}`));
            process.exit(1);
        }

        try {
            await fs.copy(templatesDir, fullPath, {
                filter: (src) => {
                    const basename = path.basename(src);
                    return !['node_modules', '.git', 'bin', 'obj', '.vs', '.idea', 'cli-test-output'].includes(basename);
                }
            });

            // Feature Flag logic would go here
            console.log(green(`\n✔ Base Architecture Deployed.`));

            // Git Init
            try {
                const { execSync } = await import('child_process');
                execSync('git init', { cwd: fullPath, stdio: 'ignore' });
                console.log(green('✔ Git Repository Initialized.'));
            } catch (e) { /* ignore */ }

            // --- SUCCESS ---
            exitAltScreen(); // Restore Terminal

            // Print Summary to Main Buffer
            console.log(green(`\n✔ APEIRON Deployed: ${bold(projectDir)}`));
            console.log('\nDeployment Summary:');
            console.log(cyan(`  cd ${projectDir}`));
            console.log(cyan(`  docker-compose up -d --build`));
            console.log(bold('\nSystems Online.'));

        } catch (err) {
            exitAltScreen();
            console.error(red('\n✖ Scaffolding Failed:'), err);
            process.exit(1);
        }
    });

program.parse();
