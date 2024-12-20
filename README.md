# UniversalCopilot

[![License](https://img.shields.io/github/license/Galygious/UniversalCopilot?style=for-the-badge)](./LICENSE)
[![Latest Release](https://img.shields.io/github/v/release/Galygious/UniversalCopilot?sort=semver&style=for-the-badge)](https://github.com/Galygious/UniversalCopilot/releases)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen?style=for-the-badge)](#) <!-- Replace # with your CI workflow badge URL -->

**UniversalCopilot** is an AI-powered Windows application that integrates an LLM-driven context menu directly into your workflow. With a simple hotkey combination and right-click, you can summarize text, translate it, generate custom completions, or perform other intelligent actions, all powered by a locally hosted Large Language Model.

## Requirements

- Windows OS
- .NET 6 or newer runtime (if not using a self-contained build)
- A locally hosted LLM endpoint (e.g., LM Studio at `http://localhost:1234`)
- The `menus.hcl` configuration file placed in the same directory as the `.exe`

For detailed installation steps, including setting up the environment and running the LLM server, check the [installation notes](./docs/installation.md).

## Usage Overview

1. **Select Text** in any application.
2. **Trigger the Menu**: Press **Ctrl+Shift** and Right-Click to open the AI-driven context menu.
3. **Choose an Action**: Summarize, translate, fill, or invoke custom actions defined in `menus.hcl`.
4. **Watch the Magic**: The response from the LLM is inserted back into your target field or application.

For more in-depth usage instructions, see the [usage guide](./docs/usage.md).

## Additional Documentation

- [Installation Notes](./docs/installation.md): Detailed steps to set up and run UniversalCopilot.
- [Usage Guide](./docs/usage.md): More examples, common commands, and workflows.
- [Advanced Topics](./docs/advanced.md): Customizing prompts, adding new actions, and integration tips.
- [Contributing](./docs/CONTRIBUTING.md): Learn how to contribute to UniversalCopilot.
- [Code of Conduct](./docs/CODE_OF_CONDUCT.md): Standards we follow to create a welcoming community.

## Useful Resources

- [LM Studio](https://lmstudio.ai/) for hosting your local LLM.
- [Microsoft .NET Documentation](https://docs.microsoft.com/dotnet/) for runtime and SDK details.
---

**Happy Coding!** With UniversalCopilot, you can unlock new levels of productivity and intelligence in your daily workflow, all while keeping your documentation organized and user-friendly.
