#!/bin/bash
set -e

# Define cleanup function
cleanup() {
    echo "🧹 [CLI-TEST] Cleaning up..."
    # Navigate back to root if we are inside a subdirectory
    cd "$PROJECT_ROOT" || true
    if [ -d "cli-test-output" ]; then
        rm -rf cli-test-output
        echo "✔ Wiped functionality test directory."
    fi
}

# Trap EXIT signal (happens on success OR failure)
trap cleanup EXIT

# Get absolute path to project root
PROJECT_ROOT=$(pwd)

echo "🚀 [CLI-TEST] Installing CLI dependencies..."
cd cli
if [ ! -f package.json ]; then
    npm init -y
    npm install commander prompts fs-extra picocolors
fi
# Ensure dependencies are installed
npm install --silent

echo "🚀 [CLI-TEST] Generating test project..."
# Force clean just in case
rm -rf "$PROJECT_ROOT/cli-test-output"

# Run generator against the templates folder
# Note: we use 'node .' instead of 'node index.js' to test entry point logic if defined, otherwise index.js
node index.js "$PROJECT_ROOT/cli-test-output"

echo "🚀 [CLI-TEST] Verifying generated artifact..."
cd "$PROJECT_ROOT/cli-test-output"

if [ ! -d "backend/dotnet" ]; then
    echo "❌ [CLI-TEST] Failed: backend/dotnet directory missing!"
    exit 1
fi

# Paranoia Check: Ensure no bin/obj folders were copied
if [ -d "backend/dotnet/src/Apeiron.Api/bin" ]; then
    echo "❌ [CLI-TEST] Failed: 'bin' directory was copied! Filter failure."
    exit 1
fi

echo "🚀 [CLI-TEST] Building generated project..."
cd backend/dotnet
dotnet build

echo "🚀 [CLI-TEST] Running tests in generated project..."
dotnet test

echo "✅ [CLI-TEST] ALL TESTS PASSED."
# Trap will handle final cleanup
