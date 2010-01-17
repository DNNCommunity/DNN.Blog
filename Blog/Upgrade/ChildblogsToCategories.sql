-- Rip Rowan 10/20/2009
--
-- This script converts all of the child blogs within a PORTAL into categories.
--
-- edited Peter Donker 14 Jan 2010
-- The script will be loaded and executed by the button on Module Options page

-- create categories
insert into {databaseOwner}{objectQualifier}blog_categories (category, slug, parentid, portalid) 
select distinct b.title, 'Default.aspx', 0, @portalid from {databaseOwner}{objectQualifier}blog_blogs b
where b.parentblogid > 0 and b.portalid = @portalid
and not exists(select * from {databaseOwner}{objectQualifier}blog_categories where title=b.title and portalid=@portalid)
GO

insert into {databaseOwner}{objectQualifier}blog_entry_categories 
      (entryid, catid) 
      select be.entryid, c.catid 
            from (select entryid, b.title 
                  from {databaseOwner}{objectQualifier}blog_entries e 
                  inner join {databaseOwner}{objectQualifier}blog_blogs b on e.blogid = b.blogid  
                  where portalid = 0) be 
      inner join {databaseOwner}{objectQualifier}blog_categories c on title = c.category
where not exists(select * from {databaseOwner}{objectQualifier}blog_entry_categories where entryid=be.entryid and catid=c.catid)
GO

update {databaseOwner}{objectQualifier}blog_entries set blogid = parentblogid
      from {databaseOwner}{objectQualifier}blog_entries e
      inner join {databaseOwner}{objectQualifier}blog_blogs b
      on e.blogid = b.blogid
      where portalid = @portalid
      and parentblogid > 0
GO

delete from {databaseOwner}{objectQualifier}blog_blogs
      where portalid = @portalid
      and parentblogid > 0
GO
	