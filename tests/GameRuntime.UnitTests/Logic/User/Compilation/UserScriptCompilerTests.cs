using GameRuntime.Logic.User.Compilation;
using Shouldly;
using Xunit;

namespace GameRuntime.UnitTests.Logic.User.Compilation;

public sealed class UserScriptCompilerTests
{
    [Fact]
    public void Compile_ShouldBuildValidUserScript()
    {
        using CompiledUserScript script = new UserScriptCompiler().Compile("return new Idle();");

        script.AssemblyName.ShouldStartWith("UserScript_");
        script.AssemblyBytes.Length.ShouldBeGreaterThan(0);
        File.Exists(script.AssemblyPath).ShouldBeTrue();
    }

    [Fact]
    public void Compile_ShouldRejectForbiddenSystemIoApi()
    {
        InvalidOperationException exception = Should.Throw<InvalidOperationException>(
            () => new UserScriptCompiler().Compile("""
                System.IO.File.ReadAllText("secret.txt");
                return new Idle();
                """));

        exception.Message.ShouldContain("SEC");
    }
}
