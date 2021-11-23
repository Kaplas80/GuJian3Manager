# Contributing to the project

Thanks for taking the time to contribute! :sparkles:

In this document you will find all the information you need to make sure
the project continues to be the high-quality product we want to be!

## Reporting issues

### Issues

When reporting a problem, be as specific as possible. Ideally, you should
provide an small snippet of code that reproduces the issue. Try to provide also
the following information:

* OS: Linux / Windows / Mac OS
* Runtime: .NET Framework, Mono, .NET Core
* Version of the product
* Stacktrace if any
* What's happening and what you expect to happen

### Features

If you want to ask for a new feature, first make sure it hasn't been reported
yet by using the search box in the issue tab. Make sure that the feature aligns
with the direction of the project.

## Pull Request

Before starting a pull request, create an issue requesting the feature you would
like to see and implement. If you are fixing a bug, create also an issue to be
able to track the problem. State that you would like to work on that. The team
will reply to the issue as soon as possible, discussing the proposal if needed.
This guarantee that later on the Pull Request we don't reject the proposal
without having a discussion first and we don't waste time.

In general, the process to create a pull request is:

1. Create an issue describing the bug or feature and state you would like to
   work on that.
2. The team will cheer you and/or discuss with you the issue.
3. Fork the project.
4. Clone your forked project and create a git branch.
5. Make the necessary code changes in as many commits as you want. The commit
   message should follow this convention:

```plain
:emoji: Short description #IssueID

Long description if needed.
```

6. Create a pull request. After reviewing your changes and making any new
   commits if needed, the team will approve and merge it.

For a complete list of emoji description see
[this repository](https://github.com/slashsBin/styleguide-git-commit-message#suggested-emojis).

## Code Guidelines

We follow the [DotNet Runtime coding style](https://github.com/dotnet/runtime/blob/master/docs/coding-guidelines/coding-style.md).

### File headers

* :heavy_check_mark: **DO** put the license in the file header with this format:

```csharp
//
// <FileName>.cs
//
// Author:
//       <AuthorName> <email@example.com>
//
// Copyright (c) <Year> <AuthorName>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
```

### Usings

* :heavy_check_mark: **DO** put the `using` inside the namespace.
* :heavy_check_mark: **DO** include all the namespaces you are using.

* :heavy_check_mark: **DO** use the `using` statement for `IDisposable` types.
