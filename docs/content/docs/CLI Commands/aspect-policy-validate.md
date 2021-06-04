+++
title = "aspect policy validate"
description = ""
weight = 8
+++

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