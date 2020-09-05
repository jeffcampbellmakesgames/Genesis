<a href="https://openupm.com/packages/com.jeffcampbellmakesgames.genesis/"><img src="https://img.shields.io/npm/v/com.jeffcampbellmakesgames.genesis?label=openupm&amp;registry_uri=https://package.openupm.com" /></a>
<img alt="GitHub issues" src="https://img.shields.io/github/issues/jeffcampbellmakesgames/Genesis?style=flat-square">[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Twitter Follow](https://img.shields.io/badge/twitter-%40stampyturtle-blue.svg?style=flat&label=Follow)](https://twitter.com/stampyturtle)

# Genesis

## About
Genesis is a general-purpose, plugin-extensible code generation framework for Unity.

## Minimum Requirenments
* Unity 2019.4.X

## Installing Genesis
Using this library in your project can be done in three ways:

### Install via OpenUPM
The package is available on the [openupm registry](https://openupm.com/). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add com.jeffcampbellmakesgames.Genesis
```

### Install via GIT URL
Using the native Unity Package Manager introduced in 2017.2, you can add this library as a package by modifying your `manifest.json` file found at `/ProjectName/Packages/manifest.json` to include it as a dependency. See the example below on how to reference it.

```
{
	"dependencies": {
		...
		"com.jeffcampbellmakesgames.genesis" : "https://github.com/jeffcampbellmakesgames/genesis.git#release/stable",
		...
	}
}
```


You will need to have Git installed and available in your system's PATH.

### Install via classic `.UnityPackage`
The latest release can be found [here](https://github.com/jeffcampbellmakesgames/Genesis/releases) as a UnityPackage file that can be downloaded and imported directly into your project's Assets folder.

## Usage

To learn more about how to use JCMG Genesis, see [here](USAGE.md) for more information.

## Support
If this is useful to you and/or you’d like to see future development and more tools in the future, please consider supporting it either by contributing to the Github projects (submitting bug reports or features and/or creating pull requests) or by buying me coffee using any of the links below. Every little bit helps!

[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/I3I2W7GX)

## Contributing

For information on how to contribute and code style guidelines, please visit [here](CONTRIBUTING.md).
