using QuikGraph;

namespace StardewCompletionist.Tasks;

public class TaskData
{
    // Notes on translating between my language and QuikGraph's terminology
    //  - Prerequiste Tasks = must be done before the target task (QuickGraph's incoming edges)
    //  - Subsequent Tasks = cannot be done until the target task is done (QuikGraph's outgoing edges)
    // A node with any incomplete incoming edges is not ready to be completed.
    private BidirectionalGraph<Task, Edge<Task>> _graph = new();

    public void AddTask(Task task, IEnumerable<Task>? prerequisiteTasks = null)
    {
        _graph.AddVertex(task);

        foreach(var prereq in prerequisiteTasks ?? Array.Empty<Task>())
        {
            _graph.AddEdge(new Edge<Task>(prereq, task));
        }        
    }

    public Task GetTask(TaskCategory category, string name)
    {
        return _graph.Vertices?
            .First(x => x.Category == category && x.Name == name)
            ?? throw new Exception($"Could not find task matching category '{category}' and name '{name}'");
    }

    public IEnumerable<Task> GetPrerequisiteTasksFor(TaskCategory category, string name)
    {
        var task = GetTask(category, name);
        return _graph.InEdges(task).Select(x => x.Source);
    }

    public IEnumerable<Task> GetSubsequentTasksFor(TaskCategory category, string name)
    {
        var task = GetTask(category, name);
        return _graph.OutEdges(task).Select(x => x.Target);
    }
}

public class Task
{
    public TaskCategory Category { get; set; } = TaskCategory.Unknown;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public bool IsDone { get; set; }
}

public enum TaskCategory
{
    Unknown = 0,
    Achievement
}
