import picocolors from 'picocolors';
import figlet from 'figlet';
import gradient from 'gradient-string';


const { red, bold } = picocolors;

export const displayBanner = () => {
    const banner = figlet.textSync('APEIRON', { font: 'Doom' });
    const coloredBanner = gradient('red', 'orange', 'gold').multiline(banner);
    console.log(coloredBanner);
    console.log(bold(red('  THE FORGE\n')));
};

export const enterAltScreen = () => process.stdout.write('\x1b[?1049h');
export const exitAltScreen = () => process.stdout.write('\x1b[?1049l');

export const cleanup = (code = 0) => {
    exitAltScreen();
    process.exit(code);
};

export const handleSigInt = () => {
    exitAltScreen();
    console.log(red('✖ Operation aborted by user.'));
    process.exit(0);
};
