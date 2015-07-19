using System;
using System.Collections.Generic;
using Mise.Core.Entities;
using Mise.Database.StorableEntities;
using Neo4jClient;

namespace Mise.Neo4J.Relationships
{
    public class EmployeeWorkPayload
    {
        public DateTimeOffset Since { get; set; }
        public string AppsAllowed { get; set; } 
    }
    public class EmployeeWorkedAtRelationship : Relationship<EmployeeWorkPayload>, IRelationshipAllowingSourceNode<EmployeeGraphNode>, IRelationshipAllowingTargetNode<RestaurantGraphNode>
    {
        public EmployeeWorkedAtRelationship(NodeReference targetNode, EmployeeWorkPayload data) : base(targetNode, data)
        {
        }

        public override string RelationshipTypeKey
        {
            get { return "EMPLOYEE_OF"; }
        }
    }
}
