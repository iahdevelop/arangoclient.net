﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client.Data;
using ArangoDB.Client.Examples.Models;
using Xunit;

namespace ArangoDB.Client.Examples.Graphs
{
    public class GraphCommand : TestDatabaseSetup
    {
        IArangoGraph Graph()
        {
            return db.Graph("SocialGraph");
        }

        GraphIdentifierResult CreateNewGraph()
        {
            var graph = Graph();

            return graph.Create(new List<EdgeDefinitionTypedData>
            {
                new EdgeDefinitionTypedData
                {
                    Collection = typeof(Follow),
                    From = new List<Type> { typeof(Person) },
                    To = new List<Type> { typeof(Person) }
                }
            });
        }

        [Fact]
        public void RemoveVertexIfMatchFailed()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var person = new Person
            {
                Age = 21,
                Name = "raoof hojat"
            };

            var inserted = graph.InsertVertex<Person>(person);

            Assert.Throws<ArangoServerException>(() => graph.RemoveVertex<Person>(person, ifMatchRev: $"{inserted.Rev}0"));
        }

        [Fact]
        public void RemoveVertex()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var person = new Person
            {
                Age = 21,
                Name = "raoof hojat"
            };

            var inserted = graph.InsertVertex<Person>(person);

            graph.RemoveVertex<Person>(person, ifMatchRev: inserted.Rev);

            var removed = graph.GetVertex<Person>(inserted.Key);

            Assert.Null(removed);
        }

        [Fact]
        public void RemoveVertexById()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var inserted = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            graph.RemoveVertexById<Person>(inserted.Key);

            var removed = graph.GetVertex<Person>(inserted.Key);

            Assert.Null(removed);
        }

        [Fact]
        public void ReplaceVertexIfMatchFailed()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var person = new Person
            {
                Age = 21,
                Name = "raoof hojat"
            };

            var inserted = graph.InsertVertex<Person>(person);

            person.Age = 33;

            Assert.Throws<ArangoServerException>(() => graph.ReplaceVertex<Person>(person, ifMatchRev: $"{inserted.Rev}0"));
        }

        [Fact]
        public void ReplaceVertex()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var person = new Person
            {
                Age = 21,
                Name = "raoof hojat"
            };

            var inserted = graph.InsertVertex<Person>(person);

            person.Age = 33;

            graph.ReplaceVertex<Person>(person, ifMatchRev: inserted.Rev);

            var replaced = graph.GetVertex<Person>(inserted.Key);

            Assert.Equal(replaced.Age, 33);
        }

        [Fact]
        public void ReplaceVertexById()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var inserted = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            graph.ReplaceVertexById<Person>(inserted.Key, new { Age = 22 });

            var replaced = graph.GetVertex<Person>(inserted.Key);

            Assert.Null(replaced.Name);
            Assert.Equal(replaced.Age, 22);
        }

        [Fact]
        public void UpdateVertexIfMatchFailed()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var person = new Person
            {
                Age = 21,
                Name = "raoof hojat"
            };

            var inserted = graph.InsertVertex<Person>(person);

            person.Age = 33;

            Assert.Throws<ArangoServerException>(() => graph.UpdateVertex<Person>(person, ifMatchRev: $"{inserted.Rev}0"));
        }

        [Fact]
        public void UpdateVertex()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var person = new Person
            {
                Age = 21,
                Name = "raoof hojat"
            };

            var inserted = graph.InsertVertex<Person>(person);

            person.Age = 33;

            graph.UpdateVertex<Person>(person, ifMatchRev: inserted.Rev);

            var updated = graph.GetVertex<Person>(inserted.Key);

            Assert.Equal(updated.Age, 33);
        }

        [Fact]
        public void UpdateVertexById()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var inserted = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            graph.UpdateVertexById<Person>(inserted.Key, new { Age = 22 });

            var updated = graph.GetVertex<Person>(inserted.Key);

            Assert.Equal(updated.Age, 22);
        }

        [Fact]
        public void InsertVertex()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            Assert.NotNull(result.Key);
        }

        [Fact]
        public void GetVertex()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var inserted = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var result = graph.GetVertex<Person>(inserted.Key);

            Assert.NotNull(result);
            Assert.NotNull(result.Key);
        }

        [Fact]
        public void GetVertexNotFound()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.GetVertex<Person>("none");

            Assert.Null(result);
        }

        [Fact]
        public void GetVertexIfMatchFailed()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var inserted = graph.InsertVertex<Person>(new Person
            {
                Age = 21,
                Name = "raoof hojat"
            });

            var vertexInfo = db.FindDocumentInfo(inserted.Id);

            Assert.NotNull(vertexInfo);
            Assert.NotNull(vertexInfo.Rev);

            Assert.Throws<ArangoServerException>(() => graph.GetVertex<Person>(inserted.Key, ifMatchRev: $"{vertexInfo.Rev}0"));
        }

        [Fact]
        public void ListEdgeDefinitions()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var list = graph.ListEdgeDefinitions();

            Assert.Equal(list.Count, 1);
            Assert.Equal(list[0], db.SharedSetting.Collection.ResolveCollectionName<Follow>());
        }

        [Fact]
        public void ExtendEdgeDefinitions()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.Edge("Relation").ExtendDefinitions(
                new string[] { db.SharedSetting.Collection.ResolveCollectionName<Host>() },
                new string[] { db.SharedSetting.Collection.ResolveCollectionName<Host>() });

            Assert.Equal(result.EdgeDefinitions.Count, 2);
        }

        [Fact]
        public void EditEdgeDefinition()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.EditEdgeDefinition<Follow, Follow>(new List<Type> { typeof(Host) }, new List<Type> { typeof(Host) });

            Assert.Equal(result.EdgeDefinitions.Count, 1);
            Assert.Equal(result.EdgeDefinitions[0].From[0], db.SharedSetting.Collection.ResolveCollectionName<Host>());
        }

        [Fact]
        public void DeleteEdgeDefinition()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.DeleteEdgeDefinition<Follow>();

            Assert.Equal(result.EdgeDefinitions.Count, 0);
        }

        [Fact]
        public void ListVertexCollections()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            graph.AddVertexCollection<Host>();

            var result = graph.ListVertexCollections();

            Assert.Equal(result.Count, 2);
            Assert.Equal(result.Except(new string[] {
            db.SharedSetting.Collection.ResolveCollectionName<Host>(),
            db.SharedSetting.Collection.ResolveCollectionName<Person>()
            }).Count(), 0);
        }

        [Fact]
        public void AddVertexCollection()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var result = graph.AddVertexCollection<Host>();

            Assert.Equal(result.OrphanCollections.Count, 1);
        }

        [Fact]
        public void RemoveVertexCollection()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            graph.AddVertexCollection<Host>();

            var result = graph.RemoveVertexCollection<Host>();

            Assert.Equal(result.OrphanCollections.Count, 0);
        }

        [Fact]
        public void Info()
        {
            var graph = Graph();

            var createdGraph = CreateNewGraph();

            var info = graph.Info();

            Assert.Equal(info.Key, createdGraph.Key);
            Assert.Equal(info.Id, createdGraph.Id);
        }

        [Fact]
        public void ListGraphs()
        {
            var graph = Graph();

            CreateNewGraph();

            var allGraphs = db.ListGraphs();

            Assert.Equal(allGraphs.Count, 1);

            Assert.Equal(allGraphs[0].Key, graph.Name);
        }

        [Fact]
        public void Create()
        {
            var graph = Graph();

            var result = CreateNewGraph();

            Assert.NotNull(result.Id);

            Assert.NotNull(result.Rev);

            Assert.Equal(result.EdgeDefinitions.Count, 1);
        }

        [Fact]
        public void Drop()
        {
            var graph = Graph();

            CreateNewGraph();

            var dropped = graph.Drop();

            Assert.True(dropped);
        }
    }
}