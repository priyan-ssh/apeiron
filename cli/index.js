#!/usr/bin/env node

import { Command } from 'commander';
import prompts from 'prompts';
import picocolors from 'picocolors';
import fs from 'fs-extra';
import path from 'path';

import { displayBanner, enterAltScreen, exitAltScreen, cleanup, handleSigInt } from './lib/ui.js';
import { runWizard, confirmOverwrite } from './lib/wizard.js';
import { generateProject } from './lib/generator.js';

const { cyan, red, bold } = picocolors;

const program = new Command();

process.on('SIGINT', handleSigInt);

// Orchestrator
const startScaffoldFlow = async (targetDirArg = null) => {
    enterAltScreen();
    displayBanner();

    try {
        let config = await runWizard(targetDirArg);

        if (!config) {
            exitAltScreen();
            console.log(red('✖ Operation aborted.'));
            process.exit(1);
        }

        const fullPath = path.resolve(process.cwd(), config.projectDir);

        if (fs.existsSync(fullPath)) {
            const { overwrite } = await confirmOverwrite(config.projectDir);
            if (!overwrite) {
                exitAltScreen();
                console.log(red('✖ Protocol halted.'));
                process.exit(1);
            }
        }

        await generateProject(config);
        exitAltScreen();

    } catch (err) {
        exitAltScreen();
        console.error(red('\n✖ Scaffolding Failed:'), err);
        process.exit(1);
    }
};

program
    .name('apeiron')
    .description('Scaffold a new Production-Ready APEIRON project')
    .version('1.0.0')
    .argument('[command]', 'Command to execute (e.g., init)')
    .action(async (commandArg) => {
        // Handle explicit 'init' command to launch wizard
        if (commandArg === 'init') {
            await startScaffoldFlow();
            return;
        }

        // Reject other arguments for now
        if (commandArg) {
            console.log(red(`✖ Unknown command: ${commandArg}`));
            console.log(cyan(`Try '${bold('apeiron')}' (Menu) or '${bold('apeiron init')}'`));
            process.exit(1);
        }

        // Main Menu
        displayBanner();
        const { action } = await prompts({
            type: 'select',
            name: 'action',
            message: 'Main Menu',
            choices: [
                { title: 'Initialize New Project', value: 'init' },
                { title: 'Help', value: 'help' },
                { title: 'Exit', value: 'exit' }
            ],
            initial: 0
        });

        if (action === 'init') {
            await startScaffoldFlow();
        } else if (action === 'help') {
            program.help();
        } else {
            console.log(red('\n...the creation awaits.'));
            process.exit(0);
        }
    });

program.parse();
