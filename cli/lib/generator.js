import fs from 'fs-extra';
import path from 'path';
import picocolors from 'picocolors';
import { fileURLToPath } from 'url';

const { cyan, green, red, bold, yellow } = picocolors;
const __dirname = path.dirname(fileURLToPath(import.meta.url));
const templatesDir = path.resolve(__dirname, '../../templates');

export const generateProject = async (config) => {
    const {
        projectDir,
        backendDirName,
        frontendDirName,
        useGit,
        selectedStack,
        features
    } = config;

    const fullPath = path.resolve(process.cwd(), projectDir);

    console.log(bold(`\n📝 Configuration Summary:`));
    console.log(`   ${bold('Project:')}   ${cyan(projectDir)}`);
    console.log(`   ${bold('Stack:')}     ${green(selectedStack.toUpperCase())}`);
    if (selectedStack !== 'frontend') console.log(`   ${bold('Backend:')}   ${cyan(backendDirName)}`);
    if (selectedStack !== 'backend') console.log(`   ${bold('Frontend:')}  ${cyan(frontendDirName)}`);
    console.log(`   ${bold('Git Init:')}  ${useGit ? green('Yes') : red('No')}`);
    console.log(`   ${bold('Modules:')}   ${features.join(', ') || 'None'}`);

    console.log(cyan(`\n🔧 Scaffolding infrastructure in ${bold(projectDir)}...`));

    // Validation
    if (!fs.existsSync(templatesDir)) {
        throw new Error(`Critical Failure: Templates not found at ${templatesDir}`);
    }

    // Prepare Directory
    if (fs.existsSync(fullPath)) {
        await fs.emptyDir(fullPath);
    } else {
        await fs.ensureDir(fullPath);
    }

    const filterFn = (src) => {
        const basename = path.basename(src);
        if (['node_modules', '.git', 'bin', 'obj', '.vs', '.idea', 'cli-test-output'].includes(basename)) return false;
        if (path.extname(src) === '.md' && basename.toLowerCase() !== 'readme.md') return false;
        return true;
    };

    const isBackend = selectedStack === 'full' || selectedStack === 'backend';
    const isFrontend = selectedStack === 'full' || selectedStack === 'frontend';

    // 1. Backend
    if (isBackend) {
        const backendSrc = path.join(templatesDir, 'backend/dotnet');
        const backendDest = path.join(fullPath, backendDirName);
        if (fs.existsSync(backendSrc)) {
            await fs.copy(backendSrc, backendDest, { filter: filterFn });
        }
    }

    // 2. Frontend
    if (isFrontend) {
        const frontendSrc = path.join(templatesDir, 'frontend/react');
        const frontendDest = path.join(fullPath, frontendDirName);
        if (fs.existsSync(frontendSrc)) {
            await fs.copy(frontendSrc, frontendDest, { filter: filterFn });
        } else {
            await fs.ensureDir(frontendDest);
        }
    }

    // 3. DevOps
    if (isBackend) {
        const dockerSrc = path.join(templatesDir, 'devops/docker-compose.yml');
        const dockerDest = path.join(fullPath, 'docker-compose.yml');
        if (fs.existsSync(dockerSrc)) {
            await fs.copy(dockerSrc, dockerDest);
        }
    }

    console.log(green(`\n✔ Architecture Deployed.`));

    // Git Init
    if (useGit) {
        await initializeGit(fullPath);
    }

    printSuccessSummary(config);
};

const initializeGit = async (fullPath) => {
    try {
        const { execSync } = await import('child_process');
        execSync('git init', { cwd: fullPath, stdio: 'ignore' });

        // Create Root .gitignore
        const gitignoreContent = `# Apeiron Root Ignore
node_modules/
dist/
build/
cli-test-output/

# IDEs
.idea/
.vscode/
*.swp
*.swo

# OS
.DS_Store
Thumbs.db

# Backend Build
**/bin/
**/obj/
**/TestResults/

# Logs
*.log
`;
        await fs.writeFile(path.join(fullPath, '.gitignore'), gitignoreContent);

        console.log(green('✔ Git Repository Initialized.'));
    } catch (e) {
        console.log(yellow('⚠ Git init failed (system dependency missing?)'));
    }
}

const printSuccessSummary = (config) => {
    const { projectDir, backendDirName, frontendDirName, selectedStack } = config;
    const isBackend = selectedStack === 'full' || selectedStack === 'backend';
    const isFrontend = selectedStack === 'full' || selectedStack === 'frontend';

    console.log(green(`\n✔ APEIRON Deployed: ${bold(projectDir)}`));
    console.log('\nStructure:');
    console.log(`  📂 ${projectDir}`);
    if (isBackend) console.log(`  ├── 📂 ${backendDirName} (.NET 10)`);
    if (isFrontend) console.log(`  ├── 📂 ${frontendDirName} (React 19)`);
    if (isBackend) console.log(`  └── 🐳 docker-compose.yml`);

    console.log('\nDeployment Summary:');
    console.log(bold(`  1. Enter Project:`));
    console.log(cyan(`     cd ${projectDir}`));

    if (isBackend) {
        console.log(bold(`\n  2. Start Core System:`));
        console.log(cyan(`     docker-compose up -d --build`));
    }

    if (isFrontend) {
        console.log(bold(`\n  3. Start Interface:`));
        console.log(cyan(`     cd ${frontendDirName}`));
        console.log(cyan(`     npm install`));
        console.log(cyan(`     npm run dev`));
    }
    console.log(bold('\nSystems Online.'));
}
