<?xml version="1.0" encoding="utf-8" ?> 
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:slash="http://purl.org/rss/1.0/modules/slash/">     <xsl:output method="xml" />
     <xsl:template match="rss/channel">
     <html>
          <head>
               <title><xsl:value-of select="title" /></title>
               <style media="all" lang="en" type="text/css">
                    .HeadChannelTitle
                    {
                         font-family:  Verdana;
                         font-size:  9pt;
                         font-weight:  bold;
                         border-width:  2px;
                         border-color:  #336699;
                         border-style:  solid;                                                  
                         width:  100%;
                         text-align:  center;
                    }
                    .HeadArticleEntry
                    {
                         border-width:  2px;
                         border-color:  #336699;
                         border-style:  solid;
                         width:  100%;
                         text-align:  left;
                    }
                    .HeadArticleTitle
                    {
                         background-color:  DarkGray;
                         color:  Black;
                         font-family:  Verdana;
                         font-size:  8pt;
                         font-weight:  bold;
                         padding-left:  5px;
                         padding-top:  5px;
                         padding-bottom:  5px;
                    }
                    .HeadArticleHeader
                    {
                         background-color:  LightGrey;
                         color:  Black;
                         font-family:  Verdana;
                         font-size:  7pt;
                         padding-left:  5px;
                         padding-top:  2px;
                         padding-bottom:  2px;
                    }
                    .HeadArticleHeader A:visited
                    {
                         color:  DarkBlue;
                         text-decoration:  none;
                    }
                    .HeadArticleHeader A:link
                    {
                         color:  DarkBlue;
                         text-decoration:  none;
                    }
                    .HeadArticleHeader A:hover
                    {
                         color:  RoyalBlue;
                         text-decoration:  underline;
                    }
                    .HeadArticleFooter
                    {
                         background-color:  LightGrey;
                         color:  Black;
                         font-family:  Verdana;
                         font-size:  7pt;
                         padding-left:  5px;
                         padding-top:  2px;
                         padding-bottom:  2px;
                    }
                    .HeadArticleFooter A:visited
                    {
                         color:  DarkBlue;
                         text-decoration:  none;
                    }
                    .HeadArticleFooter A:link
                    {
                         color:  DarkBlue;
                         text-decoration:  none;
                    }
                    .HeadArticleFooter A:hover
                    {
                         color:  RoyalBlue;
                         text-decoration:  underline;
                    }
                    .HeadArticleDescription
                    {
                         color:  #000000;
                         font-family:  Verdana;
                         font-size:  9pt;
                         padding-left:  5px;
                         padding-top:  5px;
                         padding-bottom:  5px;
                         padding-right:  5px;
                    }
               </style>
          </head>     
          <body>
               <xsl:apply-templates select="title" />
               <xsl:apply-templates select="item" />
          </body>
     </html>
     </xsl:template>
     <xsl:template match="title">
          <div class="HeadChannelTitle">
               <xsl:value-of select="text()" />
          </div>
          <br />
     </xsl:template>
     <xsl:template match="item" >
     <xsl:if test="position() &lt;= 5">
          <div class="HeadArticleEntry">
               <div class="HeadArticleTitle">
                    <xsl:value-of select="title" />
               </div>
               <div class="HeadArticleHeader">
                     <xsl:variable name="pubdate" select="pubDate"/>
        		<xsl:variable name="pubday" select="substring($pubdate, 6, 2)"/>
        		<xsl:variable name="pubmonth" select="substring($pubdate, 9, 3)"/>
        		<xsl:variable name="pubyear" select="substring($pubdate, 12, 5)"/>
        		<xsl:value-of select="concat($pubday, ',', $pubmonth, ' ', $pubyear)"/> - <a href="{link}">Link to blog</a>
        		</div>
        		<div class="HeadArticleHeader">
        		<a href="mailto:{author}">Email The Author</a>
               </div>
               <!--  ______________________________________________________________
               Template:
               Author:		
               Description:	
               
               <div class="ArticleDescription">
                    <b>Description:</b> <xsl:value-of select="description" disable-output-escaping="yes"/>
               </div>
               <div class="HeadArticleFooter">
	               <a href="{comments}">Comments (<xsl:value-of select="slash:comments"/>)</a>
               </div>
               Changes:	
               ___________________________________________________________________
               -->
               
          </div>
          <br />
		</xsl:if>
     </xsl:template>
</xsl:stylesheet>