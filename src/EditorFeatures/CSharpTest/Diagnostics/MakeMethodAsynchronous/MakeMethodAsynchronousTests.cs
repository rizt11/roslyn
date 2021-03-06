// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.MakeMethodAsynchronous;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Diagnostics.MakeMethodAsynchronous
{
    public partial class MakeMethodAsynchronousTests : AbstractCSharpDiagnosticProviderBasedUserDiagnosticTest
    {
        internal override (DiagnosticAnalyzer, CodeFixProvider) CreateDiagnosticProviderAndFixer(Workspace workspace)
            => (null, new CSharpMakeMethodAsynchronousCodeFixProvider());

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task AwaitInVoidMethodWithModifiers()
        {
            var initial =
@"using System;
using System.Threading.Tasks;

class Program 
{
    public static void Test() 
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System;
using System.Threading.Tasks;

class Program 
{
    public static async void TestAsync() 
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected, index: 1);
        }
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task AwaitInVoidMethodWithModifiers2()
        {
            var initial =
@"using System;
using System.Threading.Tasks;

class Program 
{
    public static void Test() 
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System;
using System.Threading.Tasks;

class Program 
{
    public static async Task TestAsync() 
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task AwaitInTaskMethodNoModifiers()
        {
            var initial =
@"using System;
using System.Threading.Tasks;

class Program 
{
    Task Test() 
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System;
using System.Threading.Tasks;

class Program 
{
    async Task TestAsync() 
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task AwaitInTaskMethodWithModifiers()
        {
            var initial =
@"using System;
using System.Threading.Tasks;

class Program
{
    public/*Comment*/static/*Comment*/Task/*Comment*/Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System;
using System.Threading.Tasks;

class Program
{
    public/*Comment*/static/*Comment*/async Task/*Comment*/TestAsync()
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task AwaitInLambdaFunction()
        {
            var initial =
@"using System;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        Action a = () => Console.WriteLine();
        Func<Task> b = () => [|await Task.Run(a);|]
    }
}";

            var expected =
@"using System;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        Action a = () => Console.WriteLine();
        Func<Task> b = async () => await Task.Run(a);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task AwaitInLambdaAction()
        {
            var initial =
@"using System;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        Action a = () => [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        Action a = async () => await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task BadAwaitInNonAsyncMethod()
        {
            var initial =
@"using System.Threading.Tasks;
class Program
{
    void Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System.Threading.Tasks;
class Program
{
    async void TestAsync()
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected, index: 1);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task BadAwaitInNonAsyncMethod2()
        {
            var initial =
@"using System.Threading.Tasks;
class Program
{
    Task Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System.Threading.Tasks;
class Program
{
    async Task TestAsync()
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task BadAwaitInNonAsyncMethod3()
        {
            var initial =
@"using System.Threading.Tasks;
class Program
{
    Task<int> Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System.Threading.Tasks;
class Program
{
    async Task<int> TestAsync()
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task BadAwaitInNonAsyncMethod4()
        {
            var initial =
@"using System.Threading.Tasks;
class Program
{
    int Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"using System.Threading.Tasks;
class Program
{
    async Task<int> TestAsync()
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task BadAwaitInNonAsyncMethod5()
        {
            var initial =
@"class Program
{
    void Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"class Program
{
    async void TestAsync()
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected, index: 1);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task BadAwaitInNonAsyncMethod6()
        {
            var initial =
@"class Program
{
    Task Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"class Program
{
    async Task TestAsync()
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task BadAwaitInNonAsyncMethod7()
        {
            var initial =
@"class Program
{
    Task<int> Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"class Program
{
    async Task<int> TestAsync()
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task BadAwaitInNonAsyncMethod8()
        {
            var initial =
@"class Program
{
    int Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"class Program
{
    async System.Threading.Tasks.Task<int> TestAsync()
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task BadAwaitInNonAsyncMethod9()
        {
            var initial =
@"class Program
{
    Program Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"class Program
{
    async System.Threading.Tasks.Task<Program> TestAsync()
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task BadAwaitInNonAsyncMethod10()
        {
            var initial =
@"class Program
{
    asdf Test()
    {
        [|await Task.Delay(1);|]
    }
}";

            var expected =
@"class Program
{
    async System.Threading.Tasks.Task<asdf> TestAsync()
    {
        await Task.Delay(1);
    }
}";
            await TestInRegularAndScriptAsync(initial, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task AwaitInMember()
        {
            var code =
@"using System.Threading.Tasks;

class Program
{
    var x = [|await Task.Delay(3)|];
}";
            await TestMissingInRegularAndScriptAsync(code);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task AddAsyncInDelegate()
        {
            await TestInRegularAndScriptAsync(
@"using System;
using System.Threading.Tasks;

class Program
{
    private async void method()
    {
        string content = await Task<String>.Run(delegate () {
            [|await Task.Delay(1000)|];
            return ""Test"";
        });
    }
}",
@"using System;
using System.Threading.Tasks;

class Program
{
    private async void method()
    {
        string content = await Task<String>.Run(async delegate () {
            await Task.Delay(1000);
            return ""Test"";
        });
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task AddAsyncInDelegate2()
        {
            await TestInRegularAndScriptAsync(
@"using System;
using System.Threading.Tasks;

class Program
{
    private void method()
    {
        string content = await Task<String>.Run(delegate () {
            [|await Task.Delay(1000)|];
            return ""Test"";
        });
    }
}",
@"using System;
using System.Threading.Tasks;

class Program
{
    private void method()
    {
        string content = await Task<String>.Run(async delegate () {
            await Task.Delay(1000);
            return ""Test"";
        });
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task AddAsyncInDelegate3()
        {
            await TestInRegularAndScriptAsync(
@"using System;
using System.Threading.Tasks;

class Program
{
    private void method()
    {
        string content = await Task<String>.Run(delegate () {
            [|await Task.Delay(1000)|];
            return ""Test"";
        });
    }
}",
@"using System;
using System.Threading.Tasks;

class Program
{
    private void method()
    {
        string content = await Task<String>.Run(async delegate () {
            await Task.Delay(1000);
            return ""Test"";
        });
    }
}");
        }

        [WorkItem(6477, @"https://github.com/dotnet/roslyn/issues/6477")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsAddAsync)]
        public async Task NullNodeCrash()
        {
            await TestMissingInRegularAndScriptAsync(
@"using System.Threading.Tasks;

class C
{
    static async void Main()
    {
        try
        {
            [|await|] await Task.Delay(100);
        }
        finally
        {
        }
    }
}");
        }
    }
}