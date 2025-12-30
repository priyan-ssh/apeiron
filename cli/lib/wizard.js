import prompts from 'prompts';
import picocolors from 'picocolors';
import { exitAltScreen } from './ui.js';

const { red } = picocolors;

export const runWizard = async (defaultProjectName = null) => {
    const questions = [
        {
            type: prev => defaultProjectName ? null : 'text',
            name: 'name',
            message: 'Project Name?',
            initial: 'my-apeiron-app'
        },
        {
            type: 'text',
            name: 'dir',
            message: 'Output Directory?',
            initial: prev => defaultProjectName ? `./${defaultProjectName}` : `./${prev}`
        },
        {
            type: 'select',
            name: 'stack',
            message: 'Select Architecture Stack:',
            choices: [
                { title: 'Full Stack (React + .NET)', value: 'full' },
                { title: 'Backend Only (.NET)', value: 'backend' },
                { title: 'Frontend Only (React)', value: 'frontend' }
            ],
            initial: 0
        },
        {
            type: (prev, values) => (values.stack === 'full' || values.stack === 'backend') ? 'text' : null,
            name: 'backendDir',
            message: 'Backend Directory Name?',
            initial: 'backend'
        },
        {
            type: (prev, values) => (values.stack === 'full' || values.stack === 'frontend') ? 'text' : null,
            name: 'frontendDir',
            message: 'Frontend Directory Name?',
            initial: 'frontend'
        },
        {
            type: 'confirm',
            name: 'git',
            message: 'Initialize Git Repository?',
            initial: true
        },
        {
            type: 'multiselect',
            name: 'features',
            message: 'Select Core Modules:',
            choices: [
                { title: 'Authentication (JWT)', value: 'auth', selected: true },
                { title: 'Hybrid Caching (Redis)', value: 'cache', selected: true },
                { title: 'OpenTelemetry', value: 'otel', selected: true },
                { title: 'Docker Compose', value: 'docker', selected: true }
            ]
        }
    ];

    const response = await prompts(questions);

    // Handle Cancel
    if (!response.dir && !defaultProjectName && !response.stack) {
        return null;
    }

    // Fix for when 'name' prompt is skipped because defaultProjectName is provided
    if (defaultProjectName && !response.name) {
        response.name = defaultProjectName;
    }

    return {
        projectDir: response.dir,
        backendDirName: response.backendDir || 'backend',
        frontendDirName: response.frontendDir || 'frontend',
        useGit: response.git,
        selectedStack: response.stack,
        features: response.features || []
    };
};

export const confirmOverwrite = async (projectDir) => {
    return await prompts({
        type: 'confirm',
        name: 'overwrite',
        message: `Target directory ${projectDir} exists. Overwrite?`,
        initial: false
    });
};
