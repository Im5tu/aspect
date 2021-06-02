+++
title = "Developing Locally"
description = ""
weight = 1
+++

If you wish to modify an aspect of Aspect, you'll need the source code. Clone via one of the following methods:

### Clone using SSH

```bash
git clone --recurse-submodules git@github.com:Im5tu/aspect.git
```

### Clone using Github CLI

```bash
gh repo clone Im5tu/aspect
```

### Clone using HTTPS

```bash
git clone --recurse-submodules https://github.com/Im5tu/aspect.git
```

### Building & Running tests

In order to build a development build, execute the following command:

```bash
dotnet build
```

In order to run the unit tests, execute:

```bash
dotnet test
```

The solution is designed to take care of any dependencies that are required so that contributors have the best experience in their local development environments.

Should you need to publish a new version of the CLI for your local testing, execute:

```bash
dotnet publish -r <RID> -c Release
```

You can then run one of the available [commands](/docs/getting-started/commands/).