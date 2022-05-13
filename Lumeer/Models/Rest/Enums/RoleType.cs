using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest.Enums
{
    public enum RoleType
    {
        // read resource
        Read,

        // change name, description, icon, color, folders in view
        Manage,

        // read child resources data (i.e. collection -> documents)
        DataRead,

        // edit child resources data (i.e. collection -> documents)
        DataWrite,

        // delete child resources (i.e. collection -> documents)
        DataDelete,

        // create and remove child resources (i.e. collection -> documents)
        DataContribute,

        // create new views and delete them
        ViewContribute,

        // create new collections and delete them
        CollectionContribute,

        // create new links and delete them
        LinkContribute,

        // create new projects and delete them
        ProjectContribute,

        // create new comments and delete them
        CommentContribute,

        // create, edit and delete attribute type, name, description, default attribute
        AttributeEdit,

        // set user and group permissions
        UserConfig,

        // sequences, properties, workflow, automation
        TechConfig,

        // edit perspective config in view
        PerspectiveConfig,

        // edit query in view
        QueryConfig
    }
}
