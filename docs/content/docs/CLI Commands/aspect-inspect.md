+++
title = "aspect inspect"
description = ""
weight = 2
+++

`aspect inspect` is a read-eval-print loop (REPL) interface for exploring your cloud resources.
On loading the REPL interface, you will be asked for the following information:

- Which cloud provider you wish to use
- Which regions you wish to evaluate
- Which resource you wish to evaluate

Once inside of the REPL interface and your data has been loaded, you are free to enter one or more statements to make up a policy. For example, if I was looking for resources in the region `eu-west-1`, I would enter the fragement: `input.Region == "eu-west-1"`.

Multiple policy fragments can be input on a single line using the `&&` notation. For example, if i wanted all of the resources named `default` in the region `eu-west-1`, I would write: `input.Name == "default" && input.Region == "eu-west-1"`. The REPL interface will split this into multiple lines in its generated policy statement. See the [policy syntax documentation](/docs/getting-started/policy-syntax/) for more information about the types of statements that you can write.

You also have the possibility of the using the following commands:

- `help` - Displays the properties of the type that you are exploring;
- `list` - Displays the underlying data in a table format. Note, with a large amount of resources, this might take a considerable amount of time for the formatting;
- `list N` - Displays the underlying data in a table format, limiting to the first `N` entries. eg: `list 10` would display the first 10 results;
- `refresh` - Reloads the underlying data so that you are evaluating the latest data;
- `exit` - Quits the prompt