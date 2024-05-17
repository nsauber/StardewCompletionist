using FluentAssertions;
using StardewCompletionist.Tasks;
using Xunit.Abstractions;

namespace StardewCompletionist.Tests;

public class TaskData_Tests
{
    private readonly ITestOutputHelper _output;
    private readonly TaskData _taskData;

    public TaskData_Tests(ITestOutputHelper output)
    {
        _output = output;
        _taskData = new TaskData();
    }


    //public void AddTask(Task task, IEnumerable<Task>? prerequisiteTasks = null)
    [Fact]
    public void AddTask_WithNoPrereqs_DoesNotError()
    {
        var task = new Tasks.Task();

        _taskData.AddTask(task);
    }
    [Fact]
    public void AddTask_WithOnePrereq_DoesNotError()
    {
        var task = new Tasks.Task();
        var prereq1 = new Tasks.Task();
        _taskData.AddTask(prereq1);

        _taskData.AddTask(task, new[] { prereq1 });
    }
    [Fact]
    public void AddTask_WithTwoPrereqs_DoesNotError()
    {
        var task = new Tasks.Task();
        var prereq1 = new Tasks.Task();
        var prereq2 = new Tasks.Task();
        _taskData.AddTask(prereq1);
        _taskData.AddTask(prereq2);

        _taskData.AddTask(task, new[] { prereq1, prereq2 });
    }
    [Fact]
    public void AddTask_WithAPrereqThatDoesNotExist_ThrowsException()
    {
        var task = new Tasks.Task();
        var prereq1 = new Tasks.Task();

        _taskData.Invoking(x => x.AddTask(task, new[] { prereq1 }))
            .Should().Throw<Exception>();
    }


    //public Task GetTask(TaskCategory category, string name)
    [Fact]
    public void GetTask_WhenMatchDoesNotExist_ThrowsException()
    {
        _taskData.Invoking(x => x.GetTask(TaskCategory.Achievement, "t1"))
            .Should().Throw<Exception>();
    }
    [Fact]
    public void GetTask_WhenMatchExists_ReturnsTask()
    {
        var t1 = new Tasks.Task() { Category = TaskCategory.Achievement, Name = "t1" };

        _taskData.AddTask(t1);

        _taskData.GetTask(TaskCategory.Achievement, "t1")
            .Name.Should().Be("t1");
    }


    //public IEnumerable<Task> GetPrerequisiteTasksFor(TaskCategory category, string name)
    [Fact]
    public void GetPrerequisteTasksFor_WhenMatchDoesNotExist_ThrowsException()
    {
        _taskData.Invoking(x => x.GetPrerequisiteTasksFor(TaskCategory.Achievement, "t1"))
            .Should().Throw<Exception>();
    }
    [Fact]
    public void GetPrerequisteTasksFor_WhenMatchExistsAndHasNoPrereqs_ReturnsEmptyList()
    {
        var t1 = new Tasks.Task() { Category = TaskCategory.Achievement, Name = "t1" };
        _taskData.AddTask(t1);

        var prereqs = _taskData.GetPrerequisiteTasksFor(TaskCategory.Achievement, "t1");

        prereqs.Should().BeEmpty();
    }
    [Fact]
    public void GetPrerequisteTasksFor_WhenMatchExistsAndHasMultiplePrereqs_ReturnsListWithExpectedTasks()
    {
        var t1 = new Tasks.Task() { Category = TaskCategory.Achievement, Name = "t1" };
        var prereq1 = new Tasks.Task() { Category = TaskCategory.Achievement, Name = "prereq1" };
        var prereq2 = new Tasks.Task() { Category = TaskCategory.Achievement, Name = "prereq2" };
        _taskData.AddTask(prereq1);
        _taskData.AddTask(prereq2);
        _taskData.AddTask(t1, new[] { prereq1, prereq2 });

        var prereqs = _taskData.GetPrerequisiteTasksFor(TaskCategory.Achievement, "t1");

        prereqs.Count().Should().Be(2);
        prereqs.Select(x => x.Name).Should().Contain("prereq1");
        prereqs.Select(x => x.Name).Should().Contain("prereq2");
    }


    //public IEnumerable<Task> GetSubsequentTasksFor(TaskCategory category, string name)
    [Fact]
    public void GetSubsequentTasksFor_WhenMatchDoesNotExist_ThrowsException()
    {
        _taskData.Invoking(x => x.GetSubsequentTasksFor(TaskCategory.Achievement, "t1"))
            .Should().Throw<Exception>();
    }
    [Fact]
    public void GetSubsequentTasksFor_WhenMatchExistsAndHasNoSubsequentTasks_ReturnsEmptyList()
    {
        var t1 = new Tasks.Task() { Category = TaskCategory.Achievement, Name = "t1" };
        _taskData.AddTask(t1);

        var subseqTasks = _taskData.GetSubsequentTasksFor(TaskCategory.Achievement, "t1");

        subseqTasks.Should().BeEmpty();
    }
    [Fact]
    public void GetSubsequentTasksFor_WhenMatchExistsAndHasMultipleSubsequentTasks_ReturnsListWithExpectedTasks()
    {
        var t1 = new Tasks.Task() { Category = TaskCategory.Achievement, Name = "t1" };
        var subseq1 = new Tasks.Task() { Category = TaskCategory.Achievement, Name = "subseq1" };
        var subseq2 = new Tasks.Task() { Category = TaskCategory.Achievement, Name = "subseq2" };
        _taskData.AddTask(t1);
        _taskData.AddTask(subseq1, new[] { t1 });
        _taskData.AddTask(subseq2, new[] { t1 });

        var prereqs = _taskData.GetSubsequentTasksFor(TaskCategory.Achievement, "t1");

        prereqs.Count().Should().Be(2);
        prereqs.Select(x => x.Name).Should().Contain("subseq1");
        prereqs.Select(x => x.Name).Should().Contain("subseq2");
    }

        [Fact]
    public void MultipleOperations_AddTasksWithPrereqs_CanGetTasksAndTheirDependencies()
    {
        var t1 = new Tasks.Task() { Category = TaskCategory.Achievement, Name = "t1" };
        var t2 = new Tasks.Task() { Category = TaskCategory.Achievement, Name = "t2" };

        _taskData.AddTask(t1);

        _taskData.GetTask(TaskCategory.Achievement, "t1").Name.Should().Be("t1");
        _taskData.GetPrerequisiteTasksFor(TaskCategory.Achievement, "t1").Should().BeEmpty();
        _taskData.GetSubsequentTasksFor(TaskCategory.Achievement, "t1").Should().BeEmpty();

        _taskData.AddTask(t2, new[] { t1 });

        _taskData.GetTask(TaskCategory.Achievement, "t1").Name.Should().Be("t1");
        _taskData.GetPrerequisiteTasksFor(TaskCategory.Achievement, "t1").Should().BeEmpty();
        _taskData.GetSubsequentTasksFor(TaskCategory.Achievement, "t1").Count().Should().Be(1);
        _taskData.GetSubsequentTasksFor(TaskCategory.Achievement, "t1").First().Name.Should().Be("t2");

        _taskData.GetTask(TaskCategory.Achievement, "t2").Name.Should().Be("t2");
        _taskData.GetPrerequisiteTasksFor(TaskCategory.Achievement, "t2").Count().Should().Be(1);
        _taskData.GetPrerequisiteTasksFor(TaskCategory.Achievement, "t2").First().Name.Should().Be("t1");
        _taskData.GetSubsequentTasksFor(TaskCategory.Achievement, "t2").Should().BeEmpty();
    }
}
