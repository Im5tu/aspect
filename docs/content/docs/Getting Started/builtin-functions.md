---
title: "Builtin Functions"
date: 2021-05-30T22:10:26+01:00
weight: 4
---

When building your policies, you may which to perform certain operations like seeing whether or not a string contains a specific value. Aspect provides you with a set of built in functions to help you.

All of the functions follow this format:

{{< code lang=\"tf\" >}}
functionName(input.<property>, <arg1> [, arg2..])
{{< /code >}}

The first argument to each function must be an accessor to a property on the input. This is followed by the function arguments specified in one of the functions below.

{{< alert style="warning" >}} _Note: You may not access any properties nested inside of collections at this time._ {{< /alert >}}

## Contains

### contains(string input, string value)

Returns true when the specified value string occurs within the input string, using an ordinal case insensitve comparison.

**Example**

{{< code lang=\"tf\" >}}
resource "AwsSecurityGroup"

validate {
    contains(input.Name, "contoso")
}
{{< /code >}}

### contains(string input, string value, bool caseSensitive)

Returns true when the specified value string occurs within the input string. When `caseSensitive` is false, an ordinal case insensitve comparison is used, otherwise an ordinal case sensitive comparison will be used.

**Example**

{{< code lang=\"tf\" >}}
resource "AwsSecurityGroup"

validate {
    contains(input.Name, "contoso", true)
}
{{< /code >}}

### contains(collection\<KeyValuePair> input, string key, string value)

Returns true when the KeyValuePair's key is matched with the specified key and the KeyValuePair's value is matched with the specified value. An ordinal case insensitve comparison is used for both parts of the evaluation.

**Example**

{{< code lang=\"tf\" >}}
resource "AwsSecurityGroup"

validate {
    contains(input.Tags, "Product-Group", "Test", true)
}
{{< /code >}}

### contains(collection\<KeyValuePair> input, string key, string value, bool caseSensitive)

Returns true when the KeyValuePair's key is matched with the specified key and the KeyValuePair's value is matched with the specified value. When `caseSensitive` is false, an ordinal case insensitve comparison is used, otherwise an ordinal case sensitive comparison will be used.

**Example**

{{< code lang=\"tf\" >}}
resource "AwsSecurityGroup"

validate {
    contains(input.Tags, "Product-Group", "Test", true)
}
{{< /code >}}

## EndsWith

### endsWith(string input, string value)

Returns true when the specified value string ends with the input string, using an ordinal case insensitve comparison.

**Example**

{{< code lang=\"tf\" >}}
resource "AwsSecurityGroup"

validate {
    endsWith(input.Name, "contoso")
}
{{< /code >}}

### endsWith(string input, string value, bool caseSensitive)

Returns true when the specified value string ends with the input string. When `caseSensitive` is false, an ordinal case insensitve comparison is used, otherwise an ordinal case sensitive comparison will be used.

**Example**

{{< code lang=\"tf\" >}}
resource "AwsSecurityGroup"

validate {
    endsWith(input.Name, "contoso", true)
}
{{< /code >}}

## HasKey

### hasKey(collection\<KeyValuePair> input, string key)

Returns true when the KeyValuePair's key is matched with the specified key. An ordinal case insensitve comparison is used for the evaluation.

**Example**

{{< code lang=\"tf\" >}}
resource "AwsSecurityGroup"

validate {
    hasKey(input.Tags, "Product-Group")
}
{{< /code >}}

### hasKey(collection\<KeyValuePair> input, string key, bool caseSensitive)

Returns true when the KeyValuePair's key is matched with the specified key. When `caseSensitive` is false, an ordinal case insensitve comparison is used, otherwise an ordinal case sensitive comparison will be used.

**Example**

{{< code lang=\"tf\" >}}
resource "AwsSecurityGroup"

validate {
    hasKey(input.Tags, "Product-Group", true)
}
{{< /code >}}

## Matches

### matches(string input, string regex)

Returns true when input matches the specified regex.

**Example**

{{< code lang=\"tf\" >}}
resource "Test"

validate {
    matches(input.CidrRange, "^([0-9]{1,3}\.){3}[0-9]{1,3}(\/([0-9]|[1-2][0-9]|3[0-2]))?$")
}
{{< /code >}}


## StartsWith

### startsWith(string input, string value)

Returns true when the specified value string starts with the input string, using an ordinal case insensitve comparison.

**Example**

{{< code lang=\"tf\" >}}
resource "AwsSecurityGroup"

validate {
    startsWith(input.Name, "contoso")
}
{{< /code >}}

### startsWith(string input, string value, bool caseSensitive)

Returns true when the specified value string starts with the input string. When `caseSensitive` is false, an ordinal case insensitve comparison is used, otherwise an ordinal case sensitive comparison will be used.

**Example**

{{< code lang=\"tf\" >}}
resource "AwsSecurityGroup"

validate {
    startsWith(input.Name, "contoso", true)
}
{{< /code >}}
