+++
title = "aspect watch"
description = ""
weight = 11
+++

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