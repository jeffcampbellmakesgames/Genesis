# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Releases]
### [2.4.6] - 2022-19-04
#### Added
* Added ability to load and create java ".properties" style configs

#### Changed
* Added additional logging for code-generation when loading configs

## [Releases]
### [2.4.5] - 2022-18-04
#### Fixed
* Fixed bug where the Unity project path could not include spaces when generating config contents

## [Releases]
### [2.4.4] - 2022-27-02
#### Fixed
* Fixed bug where the Unity project path could not include spaces when generating code

## [Releases]
### [2.4.3] - 2022-06-02
#### Fixed
* Fixed build compilation failure with scriptable factory class and odin integration

## [Releases]
### [2.4.2] - 2022-04-02
#### Changed
* Updated build pipeline to include dependency updates

#### Changed
* Updated factory output classes to include helper methods 

## [Releases]
### [2.4.1] - 2022-17-01
#### Fixed
* Fixed bug with factory TryGet method

## [Releases]
### [2.4.0] - 2021-11-12
#### Added
* A new factory option is available via new types `ISymbolObject` and `GenerateSymbolFactoryAttribute`. `ISymbolObject` represents a uniquely identified object by way of a `string` `Symbol` field. Any types implementing this interface that are also decorated with `GenerateSymbolFactoryAttribute` will have factory types generated for them to enable retrieving the implementing type by it's `Symbol` value.

#### Changed
* All factory classes and assets generated from `ScriptableFactoryProvider` now have `Factory` suffixed to the end of the name to make it clearer what the type is supposed to reflect. They now also have optional Odin markup that will enhance their utility, usability, and readability; this is compiled out when Odin is not available (by way of the scriptable define Odin adds to the project settings).

### [2.3.2] - 2021-07-04
#### Fixed
These issues should help resolve any operating system path issues that caused either plugins to fail to be discovered or files to be written incorrectly.
* Modified `WriteToDiskPostProcessor` so that it will replace any inapropriate path characters per-platform with the correct ones.
* Modified `GenerateOptions` and `ConfigOptions` for the Genesis.CLI so that the case-sensitive "plugins" folder name is used and removed any platform-specific path characters.
* Added `OperatingSystemTools` helper methods for determining current operating system

### [2.3.1] - 2021-07-01
#### Changed
* Modified `GetAttributes` and `HasAttributes` extension methods with optional bool parameter `canInherit` so that a user can check to see if a `ITypeSymbol` is decorated with an matching attribute or base type name.
* Adjusted interface for `ICachedNamedTypeSymbol` for `GetAttributes` and `HasAttributes` to include new optional parameter `canInherit`.

### [2.3.0] - 2021-06-15
#### Changed
* Renamed `NamedTypeSymbolInfo` to `CachedNamedTypeSymbol` and modified access to be internal. This is now exposed to users as the `ICachedNamedTypeSymbol` interface so that it can be easily changed in the future without breaking user-code.

#### Fixed
* Fixed a duplicate header on the GenesisPreferences UI.

### [2.2.2] - 2021-06-13
#### Changed
* Modified internal parallelization for Roslyn Project parsing to use async correctly.
* Modified plugin loading so that by default plugin assemblies that are not in-sync with the nuget package assemblies in use by Genesis.CLI will issue a warning and:
    * If on the same major and minor version will still be loaded.
    * If on an older major or minor version will not be loaded. These can still be force-loaded if a user passed the `load-unsafe` argument.

#### Added
* The `CHANGELOG.MD` file containing release notes is now included with the Unity package.
* Created Unity flow for third-party developers to easily expose their plugins for other users and easily install or update them in their Genesis.CLI configuration.
* Added new Genesis.CLI argument for `load-unsafe` to force load any out-of-date plugins, if any.

### [2.2.1] - 2021-06-06
#### Added
* Added additional helper methods and caching to NamedTypeSymbolInfo. All members are now lazily-loaded rather than initialized in constructor.

#### Fixed
* Modified AutoUpdate functionality so that the zip extraction only overwrites content from the zip file (does not disturb other existing custom plugins for example).

### [2.1.1] - 2021-06-05
#### Added
* Added new overload of `AssembliesConfig.FilterTypeSymbols` method that accepts a collection of `NamedTypeSymbolInfo`.

#### Fixed
* Fixed bug in `ITypeSymbol.GetArrayElementType` where the element type attempted to be returned would occasionally result in a cast exception from being cast to a `INamedTypeSymbol` when the return type is a `ITypeSymbol`.

### [2.1.0] - 2021-06-05
#### Added
* Created NamedTypeSymbolInfo as a wrapper for `INamedTypeSymbol` that caches commonly used member returns from the symbol. It's cached fields and helper methods can be used over directly accessing `INamedTypeSymbol` to improve CPU perf.
* Added auto-update detection for when a Genesis.CLI installation is out-of-date when compared to the Genesis package version. 

#### Changed
* Modified MemoryCache to cache a collection of `NamedTypeSymbolInfo` instead of `INamedTypeSymbol`. Adjusted usage in existing plugins. 
* Replaced menu item to select `Genesis.CLI.zip` file with updating existing installation, if present.
* Parallelized several loops for CPU perf improvement.

## CL Template
This template should be used for any new releases to document any changes.

### [0.0.0] - 1900-01-01
#### Added
For new features.

#### Changed
For changes in existing functionality.

#### Deprecated
For soon-to-be removed features.

#### Removed
for now removed features.

#### Fixed
For any bug fixes.

#### Security
In case of vulnerabilities.