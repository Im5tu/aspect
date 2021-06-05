+++
title = "Adding New Functions"
description = ""
weight = 2
+++

Ideally before attempting to add any builtin function, you would raise an issue on the repository so that the proposal can be discussed with the community.

Assuming that discussion has gone well, here is a list of the changes that need to be made:

1. Add the function to [BuiltinFunctions.cs](https://github.com/Im5tu/aspect/blob/main/src/Aspect.Policies/BuiltInFunctions.cs)
1. Ensure that the function is covered by [UnitTests](https://github.com/Im5tu/aspect/blob/main/tests/Aspect.Policies.Tests/BuiltInFunctionsTests.cs#L55-L75)
1. Verify that the parser understands the function signature with [tests](https://github.com/Im5tu/aspect/blob/main/tests/Aspect.Policies.Tests/CompilerServices/Parser_TypeChecking_Tests.cs) if not already covered by existing tests
1. Verify that the function generator can built the function as expected with [tests](https://github.com/Im5tu/aspect/blob/main/tests/Aspect.Policies.Tests/CompilerServices/PolicyCompilerTests.cs#L51)
1. Add the relevant [Documentation](/docs/getting-started/builtin-functions/)