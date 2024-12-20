# Installation Notes

This guide provides detailed instructions to set up and run **UniversalCopilot** on your Windows machine. UniversalCopilot is a standalone Windows application that integrates AI-powered context menu functionality using a locally hosted LLM.

---

## Prerequisites

Before running UniversalCopilot, ensure the following requirements are met:

1. **Operating System**: Windows 10 or newer (64-bit).
2. **.NET Runtime**: Install .NET 6 or newer (if using a runtime-dependent version).
   - Download the .NET Runtime from [Microsoft .NET Downloads](https://dotnet.microsoft.com/download/dotnet).
   - Alternatively, use the self-contained release, which includes the runtime.
3. **LLM Server**: A locally hosted Large Language Model (LLM) endpoint.
   - For example, [LM Studio](https://lmstudio.ai/) or similar tools.
   - The LLM server must be accessible at `http://localhost:1234`.

---

## Steps to Install and Run

### 1. Download UniversalCopilot

1. Visit the [Releases](https://github.com/Galygious/UniversalCopilot/releases) page on GitHub.
2. Download the latest version:
   - `UniversalCopilot.exe`: The main executable.
   - `menus.hcl`: The configuration file for defining the menu structure and prompts.

   Place both files in the same directory, e.g., `C:\UniversalCopilot`.

---

### 2. Install Dependencies (Optional)

If you downloaded the runtime-dependent version, ensure .NET is installed:

1. Visit the [Microsoft .NET Downloads](https://dotnet.microsoft.com/download/dotnet) page.
2. Download and install the **.NET 6+ Desktop Runtime** for your platform.

> If you downloaded the self-contained release, you can skip this step since the runtime is bundled with the `.exe`.

---

### 3. Configure the LLM Server

1. Install and run an LLM server, such as [LM Studio](https://lmstudio.ai/):
   - Download and install LM Studio.
   - Configure it to listen at `http://localhost:1234`.

2. Ensure the model you want to use is loaded and ready to process requests.

---

### 4. Run UniversalCopilot

1. Open the directory where `UniversalCopilot.exe` and `menus.hcl` are located.
2. Double-click `UniversalCopilot.exe` to start the application.
3. Look for the tray icon (a small icon in the taskbar notification area). This confirms the app is running.
4. Select some text in any application, press **Ctrl+Shift** and Right-Click to invoke the context menu.

---

## menus.hcl Configuration

The `menus.hcl` file defines the context menu structure and LLM prompts. Ensure this file is in the same directory as `UniversalCopilot.exe`. You can edit `menus.hcl` to:

- Add or modify menu items and actions.
- Customize prompts sent to the LLM.
- Add new submenus for translations, summaries, or other actions.

> See the [Usage Guide](./usage.md) for detailed instructions on editing `menus.hcl`.

---

## Troubleshooting

1. **Context Menu Doesn’t Appear**:
   - Ensure UniversalCopilot is running (check for the tray icon).
   - Verify `menus.hcl` is in the same directory as the `.exe`.
   - Confirm the global hotkey (Ctrl+Shift+Right-Click) is not being blocked by another application.

2. **No Response from LLM**:
   - Ensure the LLM server is running and accessible at `http://localhost:1234`.
   - Check that the server is loaded with a valid model and can process requests.

3. **Application Doesn’t Start**:
   - If you see a `.NET Runtime not found` error, ensure the .NET Desktop Runtime is installed.
   - For self-contained builds, verify the `.exe` isn’t blocked by antivirus software.

4. **Incorrect Text or Prompt Issues**:
   - Check the `menus.hcl` file for syntax errors.
   - Ensure `{var1}` and other variables are correctly defined.

---

## Updating UniversalCopilot

1. Visit the [Releases](https://github.com/Galygious/UniversalCopilot/releases) page.
2. Download the latest `UniversalCopilot.exe` and replace the old version in your directory.
3. Backup and review `menus.hcl` if it was customized.

---

## Support

For issues or feature requests, visit the [Issues Page](https://github.com/Galygious/UniversalCopilot/issues) on GitHub. We welcome contributions and feedback!
