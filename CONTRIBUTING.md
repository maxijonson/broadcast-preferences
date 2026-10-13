# Contributing

Thank you for considering contributing to the BroadcastPreferences plugin! We welcome contributions from the community to help improve and enhance the plugin. Please follow the guidelines below to ensure a smooth contribution process.

> This plugin was created with [Carbonoxide](https://github.com/maxijonson/carbonoxide). If you like this project's tooling and structure, consider trying it for your next Rust plugin!

## Do not edit the root `BroadcastPreferences.cs` file

The root `BroadcastPreferences.cs` file is auto-generated during the build process and should not be edited directly. Instead, please make your changes in the appropriate source files located in the `src` directory. While you may commit changes to the root file (e.g., through the `dotnet build` command), keep in mind that these changes will be overwritten the next time the project is built. **If a change is made in the root file but not in the source files, it will be lost**.

## Setup

I develop this plugin with [VS Code](https://code.visualstudio.com/download) and the [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension. You can use other IDEs, but I can't guarantee everything will work the same.

**Prerequisites**

- [dotnet](https://dotnet.microsoft.com/en-us/download/dotnet) SDK, for building the plugin and running the build scripts
- Windows, for running the Rust servers and the scripts
- [NodeJS](https://nodejs.org/), for [merging partial classes](#merging-partial-classes)

**Environments**

The plugin supports both Oxide and Carbon, so it gets tested on local Rust servers for both. I call each one an environment. An environment is a local Rust server for one framework (Oxide or Carbon) on one Steam branch, which gives 4 of them:

| Environment         | Framework | Rust branch | Port  |
| ------------------- | --------- | ----------- | ----- |
| `oxide-production`  | Oxide     | `public`    | 28015 |
| `oxide-staging`     | Oxide     | `staging`   | 28016 |
| `carbon-production` | Carbon    | `public`    | 28025 |
| `carbon-staging`    | Carbon    | `staging`   | 28026 |

Production is the `public` branch with the framework's stable build. It's what server owners are running right now. Staging is Rust's `staging` branch with the framework's staging build. It previews the next Rust update, not the next version of the plugin, so it's where you check that the plugin won't break when that update ships.

Each environment lives in its own folder under `servers` and they can all run at the same time. You don't need all 4 though. The build only targets the environments you have a server for, so if you don't care about staging, just skip the staging scripts.

**Create the servers**

1. Run `dotnet tool restore` to install [MJSU's Plugin.Merge](https://github.com/dassjosh/Plugin.Merge) and CSharpier.
2. Run `npm install` to install the [partial classes merging](#merging-partial-classes) script's dependencies.
3. Run the `update_*.bat` scripts (not the `_update_*.bat` ones!) to create the servers in the `servers` folder. Each one is a full Rust server, around 8.5 GB. Run them again whenever Rust or the framework gets an update.
4. Run each `run_*.bat` script once so the framework sets itself up, then stop the server once it's done loading. On Oxide, this writes the publicized game assemblies to `servers/oxide-*/publicized`, which the build needs. Do this again after every update so the build gets the latest assemblies.
5. In each Carbon server's `servers/carbon-*/carbon/config.json`, set `DeveloperMode` to `true`, then run the server again. Carbon then generates the developer assemblies the build needs.
6. Download [Better Chat](https://umod.org/plugins/better-chat) to `dependencies/plugins/BetterChat.cs` and run `copy_dependencies.bat`. Everything in `dependencies` gets copied into each server's `oxide` or `carbon` folder, so you can drop other plugins you want to test with in there too.

To join a server, use the F1 console with the environment's port:

```
client.connect localhost:28015
```

## Building the plugin

```bash
# Build against oxide-production, merge, format and copy to all servers
dotnet build plugin.csproj

# Same thing, but rebuilds whenever you save a file
dotnet watch build --project plugin.csproj

# Build for a specific environment (merge, format and copy to that server only)
dotnet build oxide-production.csproj
dotnet build oxide-staging.csproj
dotnet build carbon-production.csproj
dotnet build carbon-staging.csproj
```

`plugin.csproj` is the one I use while coding, but keep in mind it only compiles against oxide-production. Use `ValidateAll` to check the rest.

## Validating all environments

To check that the plugin compiles on every environment at once:

```bash
dotnet msbuild plugin.csproj -restore -t:ValidateAll
```

This runs the normal build first, then compiles `src` against each environment you have a server for, and compiles the merged `BroadcastPreferences.cs` the same way both an Oxide and a Carbon server would.

## VS Code setup

- Use the `Tasks: Run Build Task` command to see all the build tasks, instead of typing out the commands above.
- To switch between the Oxide and Carbon project contexts (e.g. to see code inside `#if CARBON` light up), use the `CSharp: Change the active document's project context` command while inside a `.cs` file and pick the environment's project.

## Merging partial classes

Plugin.Merge merges all the files in `src` into one, but at the time of writing, it just appends the partial classes one after another as it finds them. The merged plugin ends up with `partial class BroadcastPreferences` declarations sprinkled all over it, which is messy for anyone reading it (curators included).

The [`post-merge.ts`](scripts/post-merge.ts) script merges them into a single class declaration. The build runs it automatically when it finds NodeJS and the `node_modules` folder, so `npm install` is all you need. The build still works without NodeJS, but then your `BroadcastPreferences.cs` won't match the committed one, so please set it up before opening a PR.

## Conventions

The goal of having the ability to split the plugin into multiple files is to improve code organization and maintainability. Please follow the existing file structure and conventions when adding new features or making changes.

> In general, I'm pretty loose about where files go, but I'll be strict when it comes to file size.
>
> Being a Web developer primarily, where I'm used to have everything contained into their own file (each components, services, functions, etc. have their own file), **I really hate** insanely large files. Looking at Oxide plugins' code typically makes my eyes bleed everytime! 😅

- **Contained Responsibility**: Each file should have a clear and focused responsibility. Think about how the plugin may grow over time and build a structure that is future-proof against bloating single files. Avoid mixing multiple functionalities in a single file. I would rather over-simplify now than refactor later.
  - For example: All hooks are in their own file under the `Hooks` folder, rather than having one `BroadcastPreferences.Hooks.cs` file that may grow over time. Even if some of them are less than 20 lines, the idea is that the plugin may grow and more hooks may be added in the future. Growing the number of files is preferred over growing the size of existing files.
- **Folder Structure**: Organize files in a way that makes sense and try to group related files together. You don't need to break your head over this though, there may be more than one acceptable place to add your file. Just organize them in a way that makes sense and easy to guess where they belong.
- **Root Plugin Class**: Keep root class (`BroadcastPreferences`) methods and fields (i.e., those that belong to the `BroadcastPreferences` class) in the `Plugin` folder. Always prefix files under the `Plugin` folder with `BroadcastPreferences.` to signal that they belong to the root class. The `Plugin` folder is essentially the equivalent of a single large `BroadcastPreferences.cs` file split into multiple files.
- **Namespaces**: Use `BroadcastPreferencesPlugin.` followed by the top-most parent folder name under `src`. For example, the file `src/Plugin/Commands/BroadcastPreferencesCommands.cs` should use the namespace `BroadcastPreferencesPlugin.Plugin` because `Plugin` is the name of the top-most parent folder relative to `src`.
- **Build Warnings & Errors**: Always ensure that your code compiles without any warnings or errors before submitting a pull request. Treat warnings as errors to maintain a high-quality codebase
- **`FileOrder` Directive**: Use `//Define:FileOrder=<number>` at the top of files to control the order in which files are merged. Lower numbers are merged first, default is `1000`. Don't be afraid to space out numbers to allow for future insertions or even re-use existing ones. The current ordering is:
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
  - `1600` - Localization
  - `2000` - Logging

## Before Submitting a Pull Request

Before submitting a pull request, please ensure that:

- Your code follows the existing coding style and conventions used in the project.
- `dotnet msbuild plugin.csproj -restore -t:ValidateAll` passes with no warnings.
- You have tested your changes on both oxide-production and carbon-production to ensure they work as expected on both frameworks. If a Rust update is coming up, test on the staging environments too. Also test with the following plugins installed, as they are commonly used and may conflict with BroadcastPreferences:
  - [Better Chat by LaserHydra](https://umod.org/plugins/better-chat)
  - [Custom Icon by collectvood](https://umod.org/plugins/custom-icon)
