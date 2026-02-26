#!/bin/bash

# Exit on any error
set -e

# Default variables
NUGET_SOURCE="https://api.nuget.org/v3/index.json"
PROJECT_DIR="TypeCoercion"

# Ensure API key is provided
if [ -z "$1" ]; then
    echo "Error: NuGet API key is required."
    echo ""
    echo "Usage: ./publish.sh <NUGET_API_KEY>"
    exit 1
fi

NUGET_API_KEY=$1

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
