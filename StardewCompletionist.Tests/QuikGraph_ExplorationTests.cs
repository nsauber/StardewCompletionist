using QuikGraph;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.Intrinsics;
using Xunit.Abstractions;

namespace StardewCompletionist.Tests;

public class QuikGraph_ExplorationTests
{
    private readonly ITestOutputHelper _output;

    public QuikGraph_ExplorationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void SimpleBehaviorWithInts()
    {
        var edge12 = new Edge<int>(1, 2);
        var edge21 = new Edge<int>(2, 1);

        var graph = new BidirectionalGraph<int, Edge<int>>();
        
        graph.AddVerticesAndEdge(edge12);
        graph.AddVerticesAndEdge(edge21);
        WriteAllEdges(graph);
    }

    [Fact]
    public void SimpleBehaviorWithObjects()
    {
        var v1 = new TestVertex("one");
        var v2 = new TestVertex("two");

        var edge12 = new Edge<TestVertex>(v1, v2);
        var edge21 = new Edge<TestVertex>(v2, v1);
        //var edge21 = new Edge<TestVertex>(v2, new TestVertex("one"));

        var graph = new BidirectionalGraph<TestVertex, Edge<TestVertex>>();

        graph.AddVerticesAndEdge(edge12);
        graph.AddVerticesAndEdge(edge21);
        WriteAllEdges(graph);
    }

    [Fact]
    public void MoreComplicatedStructureOfObjects()
    {
        var v1 = new TestVertex("one");
        var v2 = new TestVertex("two");
        var v3 = new TestVertex("three");
        var v4 = new TestVertex("four");
        var v5 = new TestVertex("five");

        BidirectionalGraph<TestVertex, Edge<TestVertex>> graph = new BidirectionalGraph<TestVertex, Edge<TestVertex>>();

        AddVertexWithOutgoingEdges(graph, v1, new[] { v2, v3 });
        AddVertexWithOutgoingEdges(graph, v3, new[] { v4, v5 });
        AddVertexWithOutgoingEdges(graph, v4, new[] { v5 });

        WriteOutgoingEdges(graph);
    }

    private void AddVertexWithOutgoingEdges(BidirectionalGraph<TestVertex, Edge<TestVertex>> graph, TestVertex vertex, IEnumerable<TestVertex> outgoingEdgeTargets)
    {
        var edges = outgoingEdgeTargets.Select(targetVertex => new Edge<TestVertex>(vertex, targetVertex));
        graph.AddVerticesAndEdgeRange(edges);
    }

    private void WriteOutgoingEdges<T>(BidirectionalGraph<T, Edge<T>> graph)
    {
        foreach (T vertex in graph.Vertices)
        {
            _output.WriteLine($"Vertex: {vertex}");
            foreach (Edge<T> edge in graph.OutEdges(vertex))
            {
                _output.WriteLine(edge.ToString());
            }
        }
    }

    private void WriteAllEdges<T>(BidirectionalGraph<T, Edge<T>> graph)
    {
        foreach (T vertex in graph.Vertices)
        {
            _output.WriteLine($"Vertex: {vertex}");
            foreach (Edge<T> edge in graph.InEdges(vertex))
            {
                _output.WriteLine(edge.ToString());
            }
            foreach (Edge<T> edge in graph.OutEdges(vertex))
            {
                _output.WriteLine(edge.ToString());
            }
        }
    }
}

public class TestVertex
{
    public TestVertex(string name)
    {
        Name = name;
    }

    public string Name { get; set; } = string.Empty;

    public override string ToString()
    {
        return Name;
    }
}
