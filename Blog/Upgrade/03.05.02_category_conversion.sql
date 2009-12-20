-- Rip Rowan 10/20/2009
--
-- This script converts all of the child blogs within a PORTAL into categories.

-- WHO SHOULD USE THIS SCRIPT
-- If you have child blogs configured as categories and you want to convert the child blogs to the new Category feature,
-- then you need to run this script AFTER installing 03.05.02 to convert to the new category functionality.
--
-- HOW TO USE THIS SCRIPT:
-- 0.  BACK UP YOUR DATABASE.  THIS SCRIPT CANNOT BE UNDONE.
-- 1.  You will need to know the portal ID of the portal you wish to configure.
-- 2.  At the top of the script (below) you must set the value of @portalid to the ID of the portal you wish to convert
-- 3.  After setting @portalid to the correct value for your portal, save this script using a descriptive name
--     for example "myportal_03.05.02_category_conversion.sql"
-- 4.  Log into your site using your HOST account
-- 5.  Navigate to HOST > SQL
-- 6.  Choose the file you saved using the "Browse" button.  Select "Run as script".  Execute the SQL.
-- 
--
-- *** WARNING *** WARNING *** WARNING ***
-- THIS OPERATION CANNOT BE UNDONE
-- BACKUP YOUR DATABASE FIRST!!!
-- *** WARNING *** WARNING *** WARNING ***
--
-- SET YOUR PORTAL ID BELOW

declare @portalid int
set @portalid = 1

--------------------------------

delete from {databaseOwner}{objectQualifier}blog_categories

delete from {databaseOwner}{objectQualifier}blog_entry_categories

insert into {databaseOwner}{objectQualifier}blog_categories (category, parentid, portalid) select  distinct title, 0, @portalid from {databaseOwner}{objectQualifier}blog_blogs where parentblogid > 0 and portalid = @portalid

insert into {databaseOwner}{objectQualifier}blog_entry_categories 
	(entryid, catid) 
	select entryid, catid 
		from (select entryid, b.title 
			from {databaseOwner}{objectQualifier}blog_entries e 
			inner join {databaseOwner}{objectQualifier}blog_blogs b on e.blogid = b.blogid  
			where portalid = @portalid) be 
	inner join {databaseOwner}{objectQualifier}blog_categories on title = category

update {databaseOwner}{objectQualifier}blog_entries set blogid = parentblogid
	from {databaseOwner}{objectQualifier}blog_entries e
	inner join {databaseOwner}{objectQualifier}blog_blogs b
	on e.blogid = b.blogid
	where portalid = @portalid
	and parentblogid > 0

delete from {databaseOwner}{objectQualifier}blog_blogs
	where portalid = @portalid
	and parentblogid > 0