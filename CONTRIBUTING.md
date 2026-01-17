# Contributing

Thank you for considering contributing to the BroadcastPreferences plugin! We welcome contributions from the community to help improve and enhance the plugin. Please follow the guidelines below to ensure a smooth contribution process.

## Do not edit the root `BroadcastPreferences.cs` file

The root `BroadcastPreferences.cs` file is auto-generated during the build process and should not be edited directly. Instead, please make your changes in the appropriate source files located in the `src` directory. While you may commit changes to the root file (e.g., through the `dotnet build` command), keep in mind that these changes will be overwritten the next time the project is built. **If a change is made in the root file but not in the source files, it will be lost**.

## Setup

In case you're trying to replicate the dev environment I used, this plugin was originally developed using [VS Code](https://code.visualstudio.com/download) with the [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension installed. You may use other IDEs, but I can't guarantee that everything will work the same.

**Install dependencies**

This plugin uses [MJSU's Plugin Merge tool](https://github.com/dassjosh/Plugin.Merge) to merge C# files into a single plugin file for Oxide. This makes it incredibly easier to manage and organize the codebase.

You can install the Plugin Merge tool locally to the project using the following command:

```
dotnet tool restore
```

**Create a Test Server**

This repo includes a few batch files to create, update and run a local Rust server with Oxide installed for testing purposes. You can use these scripts to quickly test your changes.

1. Run the `update_production.bat` (or `update_staging.bat` for staging) script to update the local server (or create it if it doesn't exist).
2. Run the `run.bat` script to start the server.
3. Join the server in Rust using the F1 console command:
   ```
   client.connect localhost:28015
   ```

**Build the plugin**

To build the plugin and generate the `BroadcastPreferences.cs` and the `server/oxide/plugins/BroadcastPreferences.cs` file, run the following command in the root directory of the project:

```
dotnet build
```

You can also watch for changes so that the plugin is rebuilt automatically whenever you save a file:

```
dotnet watch build
```

## Conventions

The goal of having the ability to split the plugin into multiple files is to improve code organization and maintainability. Please follow the existing file structure and conventions when adding new features or making changes.

> In general, I'm pretty loose about where files go, but I'll be strict when it comes to file size.
>
> Being a Web developer primarily, where I'm used to have everything contained into their own file (each components, services, functions, etc. have their own file), **I really hate** insanely large files. Looking at Oxide plugins' code typically makes my eyes bleed everytime! 😅

- **Contained Responsibility**: Each file should have a clear and focused responsibility. Think about how the plugin may grow over time and build a structure that is future-proof against bloating single files. Avoid mixing multiple functionalities in a single file. I would rather over-simplify now than refactor later.
  - For example: All hooks are in their own file under the `Hooks` folder, rather than having one `BroadcastPreferences.Hooks.cs` file that may grow over time. Even if some of them are less than 20 lines, the idea is that the plugin may grow and more hooks may be added in the future. Growing the number of files is preferred over growing the size of existing files.
- **Folder Structure**: Organize files in a way that makes sense and try to group related files together. You don't need to break your head over this though, there may be more than one acceptable place to add your file. Just organize them in a way that makes sense and easy to guess where they belong.
- **Root Plugin Class**: Keep root class (`BroadcastPreferences`) methods and fields (i.e., those that belong to the `BroadcastPreferences` class) in the `Plugin` folder. Always prefix files under the `Plugin` folder with `BroadcastPreferences.` to signal that they belong to the root class. The `Plugin` folder is essentially the equivalent of a single large `BroadcastPreferences.cs` file split into multiple files.
- **Namespaces**: Use a namespace name for each file that matches the parent folder name under `src`. For example, the file `src/Plugin/Commands/BroadcastPreferencesCommands.cs` should use the namespace `BroadcastPreferences.Plugin` because `Plugin` is the name of the top-most parent folder relative to `src`.
- **Build Warnings & Errors**: Always ensure that your code compiles without any warnings or errors before submitting a pull request. Treat warnings as errors to maintain a high-quality codebase
- **`FileOrder` Directive**: Use `//Define:FileOrder=<number>` at the top of files to control the order in which files are merged. Lower numbers are merged first, default is `1000`. Don't be afraid to space out numbers to allow for future insertions or even re-use existing ones. The current ordering is:
  > As of Plugin.Merge v1.0.14, the FileOrder directive has an issue where all data files (separate classes) are always merged last, regardless of the specified order. Until then, it's fine to leave the order broken for data files (e.g: entities). A [PR](https://github.com/dassjosh/Plugin.Merge/pull/31) has been submitted to fix this issue.
  - `0` - Root
  - `100` - Vars
  - `200` - Hooks
  - `300` - Universal Commands
  - `301` - Universal Commands Handlers (e.g: sub-commands)
  - `305` - Console Commands
  - `306` - Console Commands Handlers (e.g: sub-commands)
  - `310` - Chat Commands
  - `311` - Chat Commands Handlers (e.g: sub-commands)
  - `1000` - Helpers
  - `1400` - Entities
  - `1500` - Configuration, Data
  - `1500` - Configuration
  - `1600` - Localization
  - `2000` - Logging

## Before Submitting a Pull Request

Before submitting a pull request, please ensure that:

- Your code follows the existing coding style and conventions used in the project.
- You have tested your changes in a local Rust server with Oxide to ensure they work as expected. Also test with the following plugins installed, as they are commonly used and may conflict with BroadcastPreferences:
  - [Better Chat by LaserHydra](https://umod.org/plugins/better-chat)
  - [Custom Icon by collectvood](https://umod.org/plugins/custom-icon)
