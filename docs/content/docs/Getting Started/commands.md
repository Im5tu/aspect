---
title: "CLI Commands"
date: 2021-05-30T22:10:26+01:00
weight: 5
---

## aspect autocomplete

_Coming soon!_

```bash
aspect autocomplete <path>
```

### Arguments

|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
|path|0|The filename or directory to validate|Current directory|true|

### Options

|Option|Alias|Description|Default|
|---|---|---|---|
||||

## aspect langserver

_Coming soon!_

```bash
aspect langserver <path>
```

### Arguments

|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
|path|0|The filename or directory to validate|Current directory|true|

### Options

|Option|Alias|Description|Default|
|---|---|---|---|
||||

## aspect policy init

Create a new policy at the specified location:

```bash
aspect policy init <filename>
```

### Arguments

|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
|filname|0|The filename where the template policy will be created||true|

## aspect policy list

List all of the policies and their resource types within the specified directory. By default, only the current directory is searched, but recursion is available.

```bash
aspect policy list <directory>
```

### Arguments

|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
|directory|0|The filename or directory to validate|Current directory|true|

### Options

|Option|Alias|Description|Default|
|---|---|---|---|
|--recursive|-r|Whether or not to recurse through child directories|false|

## aspect policy validate

Validate a specific file or directory to ensure that the policy is valid before running it using `aspect run`. If specifying a directory, the options listed below will also be available.

```bash
aspect policy validate <path> [-r|--recursive] [--failed-only]
```

### Arguments

|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
|path|0|The filename or directory to validate|Current directory|true|

### Options

|Option|Alias|Description|Default|
|---|---|---|---|
|--recursive|-r|Whether or not to recurse through child directories|false|
|--failed-only||Displays only invalid policies. No output is shown is all policies are valid|false|

## aspect inspect

_Coming soon! A REPL instance to execute commands against cloud resources for verification_

```bash
aspect inspect <path>
```

### Arguments

|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
||||

### Options

|Option|Alias|Description|Default|
|---|---|---|---|
||||

## aspect run

_Coming soon!_

```bash
aspect run <path>
```

### Arguments

|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
|path|0|The filename or directory to validate|Current directory|true|

### Options

|Option|Alias|Description|Default|
|---|---|---|---|
||||

## aspect watch

Watch a specific directory for changes and validate them as the files change.

```bash
aspect watch <directory>
```

### Arguments

|Option|Position|Description|Default|Required?|
|---|---|---|---|---|
|directory|0|The directory to watch for changes|Current directory|true|

### Options

|Option|Alias|Description|Default|
|---|---|---|---|
|--delay|-r|The period of time in milliseconds that needs to elapse between changes to a file.|500|
