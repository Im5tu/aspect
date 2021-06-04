---
title: "Builtin Functions"
date: 2021-05-30T22:10:26+01:00
weight: 4
---

When building your policies, you may which to perform certain operations like seeing whether or not a string contains a specific value. Aspect provides you with a set of built in functions to help you.

All of the functions follow this format:

```
functionName(input.<property>, <arg1> [, arg2..])
```

The first argument to each function must be an accessor to a property on the input. This is followed by the function arguments specified in one of the functions below.

_Note: You may not access any properties nested inside of collections at this time._

## contains(string input, string value)

Returns a true when the specified value string occurs within the input string, using an ordinal case insensitve comparison.

**Example**

```
resource "AwsSecurityGroup"

validate {
    contains(input.Name, "contoso")
}
```

## contains(string input, string value, bool caseSensitive)

Returns a true when the specified value string occurs within the input string. When `caseSensitive` is false, an ordinal case insensitve comparison is used, otherwise an ordinal case sensitive comparison will be used.

**Example**

```
resource "AwsSecurityGroup"

validate {
    contains(input.Name, "contoso", true)
}
```

## startsWith(string input, string value)

Returns a true when the specified value string starts with the input string, using an ordinal case insensitve comparison.

**Example**

```
resource "AwsSecurityGroup"

validate {
    startsWith(input.Name, "contoso")
}
```

## startsWith(string input, string value, bool caseSensitive)

Returns a true when the specified value string starts with the input string. When `caseSensitive` is false, an ordinal case insensitve comparison is used, otherwise an ordinal case sensitive comparison will be used.

**Example**

```
resource "AwsSecurityGroup"

validate {
    startsWith(input.Name, "contoso", true)
}
```

## endsWith(string input, string value)

Returns a true when the specified value string ends with the input string, using an ordinal case insensitve comparison.

**Example**

```
resource "AwsSecurityGroup"

validate {
    endsWith(input.Name, "contoso")
}
```

## endsWith(string input, string value, bool caseSensitive)

Returns a true when the specified value string ends with the input string. When `caseSensitive` is false, an ordinal case insensitve comparison is used, otherwise an ordinal case sensitive comparison will be used.

**Example**

```
resource "AwsSecurityGroup"

validate {
    endsWith(input.Name, "contoso", true)
}
```
