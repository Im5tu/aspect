---
title: "Installation"
date: 2021-05-30T22:10:26+01:00
weight: 1
---

The fastest way to get started with Aspect is to use the Docker image from Docker Hub:

```bash
docker pull im5tu/aspect
```

Once pulled, you can run the docker image like so:

```bash
docker run -it --rm im5tu/aspect
```

This will start a new command line with the `aspect` command available so you can then run one of the available [commands](/docs/getting-started/commands/) straight away. Depending which cloud resource you intend to use, you may also need to mount credential folders. You can view the relevant configuration for [AWS](/docs/aws/configuration/) and [Azure](/docs/azure/configuration/).

You can view the latest tags directly on Docker Hub: [here](https://hub.docker.com/repository/docker/im5tu/aspect/tags?page=1&ordering=last_updated)

If docker images aren't your thing, you may also either download the pre-compiled binaries for your infrastructure, or build the CLI from source.

## Compiled Binaries

For the compiled binaries, head over to the [Aspect Releases page](https://github.com/Im5tu/aspect/releases) and download the pre-compiled binaries for the version that you wish.

## Building from source

Building from source is the best way if you wish to modify an aspect of Aspect. You will need to clone via one of the following methods:

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