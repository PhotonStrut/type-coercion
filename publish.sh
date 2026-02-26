#!/bin/bash

# Exit on any error
set -e

# Default variables
NUGET_SOURCE="https://api.nuget.org/v3/index.json"
PROJECT_DIR="TypeCoercion"

# Ensure API key is provided via environment variable
if [ -z "$NUGET_API_KEY" ]; then
    echo "Error: NUGET_API_KEY environment variable is not set."
    echo ""
    echo "Set it by adding to your shell profile (~/.zshrc):"
    echo "  export NUGET_API_KEY=\"your-api-key-here\""
    echo ""
    echo "Or set it inline for a single run:"
    echo "  NUGET_API_KEY=\"your-api-key-here\" ./publish.sh"
    exit 1
fi

echo "Packing $PROJECT_DIR for release..."
dotnet pack $PROJECT_DIR -c Release

# Find the most recently generated .nupkg file
NUPKG_FILE=$(ls -t $PROJECT_DIR/bin/Release/*.nupkg 2>/dev/null | head -1)

if [ -z "$NUPKG_FILE" ]; then
    echo "Error: Could not find the generated .nupkg file!"
    exit 1
fi

echo "Publishing $NUPKG_FILE to NuGet..."
dotnet nuget push "$NUPKG_FILE" --api-key "$NUGET_API_KEY" --source "$NUGET_SOURCE" --skip-duplicate

echo "Successfully published!"
