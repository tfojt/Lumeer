using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    /*{
    "linkTypeId": "626ac18247fe0b7f5318c012",
    "documentIds": [ "621fb8f9d508224e0888e797", "621fb8f9d508224e0888e7a8" ],
    "id": "626ad23447fe0b7f5318c1b0",
    "creationDate": 1651167796419,
    "updateDate": null,
    "createdBy": "620fa0c48e43bf296c088962",
    "updatedBy": null,
    "dataVersion": 0,
    "data": { "_id": "626ad23447fe0b7f5318c1b0" },
    "commentsCount": 0
    }*/
    public class Link
    {
        public string LinkTypeId { get; set; }
        public List<string> DocumentIds { get; set; }
        public string Id { get; set; }
        public long CreationDate { get; set; }
        public long? UpdateDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int? DataVersion { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public int? CommentsCount { get; set; }
    }
}
