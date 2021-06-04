+++
title = "aspect policy list"
description = ""
weight = 7
+++

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