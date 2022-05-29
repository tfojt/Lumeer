using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    /*{
    "name": "Issues Features",
    "collectionIds": [ "621fb8f9d508224e0888e794", "621fb8f9d508224e0888e795" ],
    "attributes": [],
    "rules": {},
    "permissions": {
    "users": [],
      "groups": []
    },
    "permissionsType": null,
    "id": "621fb8f9d508224e0888e796",
    "version": 2,
    "lastAttributeNum": 0,
    "linksCount": 13
    }*/
    public class LinkType
    {
        public string Name { get; set; }
        public List<string> CollectionIds { get; set; }
        public object Attributes { get; set; }
        public object Rules { get; set; }
        public object Permissions { get; set; }
        public object PermissionsType { get; set; }
        public string Id { get; set; }
        public int Version { get; set; }
        public int LastAttributeNum { get; set; }
        public int? LinksCount { get; set; }
    }
}
