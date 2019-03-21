#region license
/*
Copyright 2005 - 2019 Advantage Solutions, s. r. o.

This file is part of ORIGAM (http://www.origam.org).

ORIGAM is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ORIGAM is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with ORIGAM. If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using Origam.Schema;
using Origam.Schema.WorkflowModel;
using Microsoft.Msagl.Drawing;
using System.Collections.Generic;

namespace Origam.Workbench.Diagram
{
	/// <summary>
	/// Summary description for DiagramFactory.
	/// </summary>
	public class DiagramFactory
	{
		#region Constructors
		public DiagramFactory(Graph graph)
		{
            this.Graph = graph;
		}
		#endregion

		#region Properties
		private Graph _graph;
		public Graph Graph 
		{
			get
			{
                return _graph;
			}
			set
			{
                _graph = value;
			}
		}
		#endregion

		#region Public Methods
		public void DrawDiagram(ISchemaItem item)
		{
			if(item is IWorkflowBlock)
			{
				DrawWorkflowDiagram(item as IWorkflowBlock, null);
			}
			else
			{
				DrawUniSchemaDiagram(item);
			}
		}
		#endregion

		#region Private Methods
        private Node AddBasicShape(string id,string label)
        {
            return AddBasicShape(id, label, null);
        }

		private Node AddBasicShape(string id, string label, Subgraph subgraph)
		{
			Node shape = this.Graph.AddNode(id);
            shape.LabelText = label;
            if (subgraph != null)
            {
                subgraph.AddNode(shape);
            }
			return shape;
		}

		#region Workflow Diagram
		private Subgraph DrawWorkflowDiagram(IWorkflowBlock block, Subgraph parentSubgraph)
		{
            Subgraph subgraph = new Subgraph(block.NodeId);
            subgraph.LabelText = block.Name;
            if (parentSubgraph == null)
            {
                this.Graph.RootSubgraph.AddSubgraph(subgraph);
            }
            else
            {
                parentSubgraph.AddSubgraph(subgraph);
            }
            IDictionary<Key, Node> ht = new Dictionary<Key, Node>();
			// root shape
			//Node blockShape = this.AddBasicShape(block.Name, subgraph);
			//ht.Add(block.PrimaryKey, blockShape);

			foreach(IWorkflowStep step in block.ChildItemsByType(AbstractWorkflowStep.ItemTypeConst))
			{
                IWorkflowBlock subBlock = step as IWorkflowBlock;
                if (subBlock == null)
                {
                    Node shape = this.AddBasicShape(step.NodeId, step.Name, subgraph);
                    ht.Add(step.PrimaryKey, shape);
                }
                else
                {
                    Node shape = DrawWorkflowDiagram(subBlock, subgraph);
                    ht.Add(step.PrimaryKey, shape);
                }
			}

			// add connections
			foreach(IWorkflowStep step in block.ChildItemsByType(AbstractWorkflowStep.ItemTypeConst))
			{
				Node destinationShape = ht[step.PrimaryKey];
				if(destinationShape == null) throw new NullReferenceException(ResourceUtils.GetString("ErrorDestinationShapeNotFound"));
				int i = 0;
				foreach(WorkflowTaskDependency dependency in step.ChildItemsByType(WorkflowTaskDependency.ItemTypeConst))
				{
					Node sourceShape = ht[dependency.Task.PrimaryKey];
					if(sourceShape == null) throw new NullReferenceException(ResourceUtils.GetString("ErrorSourceShapeNotFound"));

					this.Graph.AddEdge(sourceShape.Id,
                        destinationShape.Id);
					i++;
				}

				if(i==0)
				{
					// no connections, we set the connection to the root block
                    //this.Graph.AddEdge(blockShape.Id, destinationShape.Id);
				}
			}
            return subgraph;
        }
		#endregion

		#region Uni Diagram
		private void DrawUniSchemaDiagram(ISchemaItem item)
		{
			DrawUniShape(item, null);
		}

		private void DrawUniShape(ISchemaItem schemaItem, Node parentShape)
		{
			Node shape = this.AddBasicShape(schemaItem.NodeId, schemaItem.Name);
			if(parentShape != null)
			{
				this.Graph.AddEdge(shape.Id, parentShape.Id);
			}
			foreach(ISchemaItem child in schemaItem.ChildItems)
			{
				DrawUniShape(child, shape);
			}
		}
		#endregion
		#endregion
	}
}
