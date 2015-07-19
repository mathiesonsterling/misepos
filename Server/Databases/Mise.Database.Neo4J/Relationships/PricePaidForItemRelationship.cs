using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Database.StorableEntities;
using Mise.Database.StorableEntities.Inventory;
using Mise.Neo4J.Relationships;
using Neo4jClient;

namespace Mise.Neo4J.Relationships
{
    public class PricePaidForItemPayload
    {
        public decimal Price { get; set; } 
    }

    public class PricePaidForItemRelationship : Relationship<PricePaidForItemPayload>, 
        IRelationshipAllowingSourceNode<VendorBeverageLineItemGraphNode>, 
        IRelationshipAllowingTargetNode<RestaurantGraphNode>
    {
        public PricePaidForItemRelationship(NodeReference targetNode, PricePaidForItemPayload data) : base(targetNode, data)
        {
        }

        public override string RelationshipTypeKey
        {
            get { return "PAID_FOR_ITEM"; }
        }
    }
}