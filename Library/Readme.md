<a name='assembly'></a>
# ExpressionTreeToString

## Contents

- [DynamicCypherWriterVisitor](#T-ExpressionTreeToString-DynamicCypherWriterVisitor 'ExpressionTreeToString.DynamicCypherWriterVisitor')
  - [#ctor()](#M-ExpressionTreeToString-DynamicCypherWriterVisitor-#ctor-System-Collections-Generic-Dictionary{System-String,System-String},System-Object,OneOf-OneOf{System-String,System-Nullable{ZSpitz-Util-Language}},System-Boolean- 'ExpressionTreeToString.DynamicCypherWriterVisitor.#ctor(System.Collections.Generic.Dictionary{System.String,System.String},System.Object,OneOf.OneOf{System.String,System.Nullable{ZSpitz.Util.Language}},System.Boolean)')
- [ExpressionExtension](#T-ExpressionTreeToString-ExpressionExtension 'ExpressionTreeToString.ExpressionExtension')
  - [WordInDoubleQuotes()](#M-ExpressionTreeToString-ExpressionExtension-WordInDoubleQuotes 'ExpressionTreeToString.ExpressionExtension.WordInDoubleQuotes')
- [FactoryMethodsWriterVisitor](#T-ExpressionTreeToString-FactoryMethodsWriterVisitor 'ExpressionTreeToString.FactoryMethodsWriterVisitor')
  - [writeMethodCall(args)](#M-ExpressionTreeToString-FactoryMethodsWriterVisitor-writeMethodCall-System-String,System-Collections-IEnumerable- 'ExpressionTreeToString.FactoryMethodsWriterVisitor.writeMethodCall(System.String,System.Collections.IEnumerable)')
- [Runner](#T-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-RunnerFactory-Runner 'System.Text.RegularExpressions.Generated.WordInDoubleQuotes_0.RunnerFactory.Runner')
  - [Scan(inputSpan)](#M-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-RunnerFactory-Runner-Scan-System-ReadOnlySpan{System-Char}- 'System.Text.RegularExpressions.Generated.WordInDoubleQuotes_0.RunnerFactory.Runner.Scan(System.ReadOnlySpan{System.Char})')
  - [TryFindNextPossibleStartingPosition(inputSpan)](#M-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-RunnerFactory-Runner-TryFindNextPossibleStartingPosition-System-ReadOnlySpan{System-Char}- 'System.Text.RegularExpressions.Generated.WordInDoubleQuotes_0.RunnerFactory.Runner.TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})')
  - [TryMatchAtCurrentPosition(inputSpan)](#M-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-RunnerFactory-Runner-TryMatchAtCurrentPosition-System-ReadOnlySpan{System-Char}- 'System.Text.RegularExpressions.Generated.WordInDoubleQuotes_0.RunnerFactory.Runner.TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})')
- [RunnerFactory](#T-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-RunnerFactory 'System.Text.RegularExpressions.Generated.WordInDoubleQuotes_0.RunnerFactory')
  - [CreateInstance()](#M-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-RunnerFactory-CreateInstance 'System.Text.RegularExpressions.Generated.WordInDoubleQuotes_0.RunnerFactory.CreateInstance')
- [Utilities](#T-System-Text-RegularExpressions-Generated-Utilities 'System.Text.RegularExpressions.Generated.Utilities')
  - [s_defaultTimeout](#F-System-Text-RegularExpressions-Generated-Utilities-s_defaultTimeout 'System.Text.RegularExpressions.Generated.Utilities.s_defaultTimeout')
  - [s_hasTimeout](#F-System-Text-RegularExpressions-Generated-Utilities-s_hasTimeout 'System.Text.RegularExpressions.Generated.Utilities.s_hasTimeout')
  - [IsWordChar()](#M-System-Text-RegularExpressions-Generated-Utilities-IsWordChar-System-Char- 'System.Text.RegularExpressions.Generated.Utilities.IsWordChar(System.Char)')
- [WordInDoubleQuotes_0](#T-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0 'System.Text.RegularExpressions.Generated.WordInDoubleQuotes_0')
  - [#ctor()](#M-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-#ctor 'System.Text.RegularExpressions.Generated.WordInDoubleQuotes_0.#ctor')
  - [Instance](#F-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-Instance 'System.Text.RegularExpressions.Generated.WordInDoubleQuotes_0.Instance')
- [WriterVisitorBase](#T-ExpressionTreeToString-WriterVisitorBase 'ExpressionTreeToString.WriterVisitorBase')
  - [WriteNode(o,parameterDeclaration,blockType)](#M-ExpressionTreeToString-WriterVisitorBase-WriteNode-System-String,System-Object,System-Boolean,System-Object- 'ExpressionTreeToString.WriterVisitorBase.WriteNode(System.String,System.Object,System.Boolean,System.Object)')

<a name='T-ExpressionTreeToString-DynamicCypherWriterVisitor'></a>
## DynamicCypherWriterVisitor `type`

##### Namespace

ExpressionTreeToString

##### Summary



##### See Also

- [ExpressionTreeToString.DynamicLinqWriterVisitor](#T-ExpressionTreeToString-DynamicLinqWriterVisitor 'ExpressionTreeToString.DynamicLinqWriterVisitor')

<a name='M-ExpressionTreeToString-DynamicCypherWriterVisitor-#ctor-System-Collections-Generic-Dictionary{System-String,System-String},System-Object,OneOf-OneOf{System-String,System-Nullable{ZSpitz-Util-Language}},System-Boolean-'></a>
### #ctor() `constructor`

##### Summary



##### Parameters

This constructor has no parameters.

##### See Also

- [ExpressionTreeToString.DynamicLinqWriterVisitor](#T-ExpressionTreeToString-DynamicLinqWriterVisitor 'ExpressionTreeToString.DynamicLinqWriterVisitor')

<a name='T-ExpressionTreeToString-ExpressionExtension'></a>
## ExpressionExtension `type`

##### Namespace

ExpressionTreeToString

<a name='M-ExpressionTreeToString-ExpressionExtension-WordInDoubleQuotes'></a>
### WordInDoubleQuotes() `method`

##### Parameters

This method has no parameters.

##### Remarks

Pattern:

```
\\\\"(?&lt;word&gt;\\w+)\\\\"
```

Explanation:

```
○ Match the string "\\\"".
```

<a name='T-ExpressionTreeToString-FactoryMethodsWriterVisitor'></a>
## FactoryMethodsWriterVisitor `type`

##### Namespace

ExpressionTreeToString

<a name='M-ExpressionTreeToString-FactoryMethodsWriterVisitor-writeMethodCall-System-String,System-Collections-IEnumerable-'></a>
### writeMethodCall(args) `method`

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| args | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The arguments to write. If a tuple of string and node type, will write as single node. If a tuple of string and property type, will write as multiple nodes. |

<a name='T-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-RunnerFactory-Runner'></a>
## Runner `type`

##### Namespace

System.Text.RegularExpressions.Generated.WordInDoubleQuotes_0.RunnerFactory

##### Summary

Provides the runner that contains the custom logic implementing the specified regular expression.

<a name='M-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-RunnerFactory-Runner-Scan-System-ReadOnlySpan{System-Char}-'></a>
### Scan(inputSpan) `method`

##### Summary

Scan the `inputSpan` starting from base.runtextstart for the next match.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| inputSpan | [System.ReadOnlySpan{System.Char}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.ReadOnlySpan 'System.ReadOnlySpan{System.Char}') | The text being scanned by the regular expression. |

<a name='M-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-RunnerFactory-Runner-TryFindNextPossibleStartingPosition-System-ReadOnlySpan{System-Char}-'></a>
### TryFindNextPossibleStartingPosition(inputSpan) `method`

##### Summary

Search `inputSpan` starting from base.runtextpos for the next location a match could possibly start.

##### Returns

true if a possible match was found; false if no more matches are possible.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| inputSpan | [System.ReadOnlySpan{System.Char}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.ReadOnlySpan 'System.ReadOnlySpan{System.Char}') | The text being scanned by the regular expression. |

<a name='M-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-RunnerFactory-Runner-TryMatchAtCurrentPosition-System-ReadOnlySpan{System-Char}-'></a>
### TryMatchAtCurrentPosition(inputSpan) `method`

##### Summary

Determine whether `inputSpan` at base.runtextpos is a match for the regular expression.

##### Returns

true if the regular expression matches at the current position; otherwise, false.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| inputSpan | [System.ReadOnlySpan{System.Char}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.ReadOnlySpan 'System.ReadOnlySpan{System.Char}') | The text being scanned by the regular expression. |

<a name='T-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-RunnerFactory'></a>
## RunnerFactory `type`

##### Namespace

System.Text.RegularExpressions.Generated.WordInDoubleQuotes_0

##### Summary

Provides a factory for creating [RegexRunner](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.RegularExpressions.RegexRunner 'System.Text.RegularExpressions.RegexRunner') instances to be used by methods on [Regex](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.RegularExpressions.Regex 'System.Text.RegularExpressions.Regex').

<a name='M-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-RunnerFactory-CreateInstance'></a>
### CreateInstance() `method`

##### Summary

Creates an instance of a [RegexRunner](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.RegularExpressions.RegexRunner 'System.Text.RegularExpressions.RegexRunner') used by methods on [Regex](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.RegularExpressions.Regex 'System.Text.RegularExpressions.Regex').

##### Parameters

This method has no parameters.

<a name='T-System-Text-RegularExpressions-Generated-Utilities'></a>
## Utilities `type`

##### Namespace

System.Text.RegularExpressions.Generated

##### Summary

Helper methods used by generated [Regex](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.RegularExpressions.Regex 'System.Text.RegularExpressions.Regex')-derived implementations.

<a name='F-System-Text-RegularExpressions-Generated-Utilities-s_defaultTimeout'></a>
### s_defaultTimeout `constants`

##### Summary

Default timeout value set in [AppContext](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.AppContext 'System.AppContext'), or [InfiniteMatchTimeout](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.RegularExpressions.Regex.InfiniteMatchTimeout 'System.Text.RegularExpressions.Regex.InfiniteMatchTimeout') if none was set.

<a name='F-System-Text-RegularExpressions-Generated-Utilities-s_hasTimeout'></a>
### s_hasTimeout `constants`

##### Summary

Whether [s_defaultTimeout](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.RegularExpressions.Generated.Utilities.s_defaultTimeout 'System.Text.RegularExpressions.Generated.Utilities.s_defaultTimeout') is non-infinite.

<a name='M-System-Text-RegularExpressions-Generated-Utilities-IsWordChar-System-Char-'></a>
### IsWordChar() `method`

##### Summary

Determines whether the character is part of the [\w] set.

##### Parameters

This method has no parameters.

<a name='T-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0'></a>
## WordInDoubleQuotes_0 `type`

##### Namespace

System.Text.RegularExpressions.Generated

##### Summary

Custom [Regex](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.RegularExpressions.Regex 'System.Text.RegularExpressions.Regex')-derived type for the WordInDoubleQuotes method.

<a name='M-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-#ctor'></a>
### #ctor() `constructor`

##### Summary

Initializes the instance.

##### Parameters

This constructor has no parameters.

<a name='F-System-Text-RegularExpressions-Generated-WordInDoubleQuotes_0-Instance'></a>
### Instance `constants`

##### Summary

Cached, thread-safe singleton instance.

<a name='T-ExpressionTreeToString-WriterVisitorBase'></a>
## WriterVisitorBase `type`

##### Namespace

ExpressionTreeToString

<a name='M-ExpressionTreeToString-WriterVisitorBase-WriteNode-System-String,System-Object,System-Boolean,System-Object-'></a>
### WriteNode(o,parameterDeclaration,blockType) `method`

##### Summary

Write a string-rendering of an expression or other type used in expression trees

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| o | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Object to be rendered |
| parameterDeclaration | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') | For ParameterExpression, this is a parameter declaration |
| blockType | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | For BlockExpression, sets the preferred block type |
