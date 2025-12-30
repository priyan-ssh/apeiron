#!/usr/bin/env node

import { Command } from 'commander';
import prompts from 'prompts';
import fs from 'fs-extra';
import path from 'path';
import picocolors from 'picocolors';
import { fileURLToPath } from 'url';

const { cyan, green, red, bold } = picocolors;
const __dirname = path.dirname(fileURLToPath(import.meta.url));

const program = new Command();

program
    .name('create-apeiron')
    .description(bold('Friday Protocol: APEIRON Construction Kit'))
    .version('1.0.0')
    .argument('[project-directory]', 'Directory to create the project in')
    .action(async (targetDir) => {
        console.log(bold(cyan('\n🚀 Initializing APEIRON Protocol...')));

        let projectDir = targetDir;

        if (!projectDir) {
            const response = await prompts({
                type: 'text',
                name: 'dir',
                message: 'Where should we deploy the stack?',
                initial: 'my-apeiron-app'
            });
            projectDir = response.dir;
        }

        if (!projectDir) {
            console.log(red('✖ Operation aborted. We need a target, sir.'));
            process.exit(1);
        }

        const fullPath = path.resolve(process.cwd(), projectDir);
        const templatesDir = path.resolve(__dirname, '../templates');

        if (fs.existsSync(fullPath)) {
            const response = await prompts({
                type: 'confirm',
                name: 'overwrite',
                message: `Target directory ${projectDir} already exists. Wipe it?`,
                initial: false
            });

            if (!response.overwrite) {
                console.log(red('✖ Protocol halted. Preserving existing assets.'));
                process.exit(1);
            }

            console.log(cyan(`\n🧹 Cleaning sector ${projectDir}...`));
            await fs.emptyDir(fullPath);
        } else {
            await fs.ensureDir(fullPath);
        }

        console.log(cyan(`\n📦 Injecting core templates from ${templatesDir}...`));

        // Check if templates exist
        if (!fs.existsSync(templatesDir)) {
            console.log(red(`✖ Critical Failure: Templates not found at ${templatesDir}`));
            console.log(red('  Run "npm run build" or ensure templates directory exists.'));
            process.exit(1);
        }

        try {
            await fs.copy(templatesDir, fullPath, {
                filter: (src) => {
                    const basename = path.basename(src);
                    return basename !== 'node_modules'
                        && basename !== '.git'
                        && basename !== 'bin'
                        && basename !== 'obj'
                        && basename !== '.vs'
                        && basename !== '.idea';
                }
            });

            console.log(green(`\n✔ Deployment Complete at ${fullPath}`));

            try {
                console.log(cyan('\n📝 Initializing Git repository...'));
                const { execSync } = await import('child_process');
                execSync('git init', { cwd: fullPath, stdio: 'ignore' });
                console.log(green('✔ Git initialized.'));
            } catch (e) {
                console.log(picocolors.yellow('⚠ precise git init failed, skipping.'));
            }

            console.log('\nFriday Protocol recommends:');
            console.log(cyan(`  cd ${projectDir}`));
            console.log(cyan(`  docker-compose up -d --build`));
            console.log(bold('\nSystems Online.'));

        } catch (err) {
            console.error(red('\n✖ Deployment Failed:'), err);
            process.exit(1);
        }
    });

program.parse();
