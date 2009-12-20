<?xml version="1.0" encoding="utf-8" ?> 
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:slash="http://purl.org/rss/1.0/modules/slash/">     <xsl:output method="xml" />
     <xsl:template match="rss/channel">
     <html>
          <head>
               <title><xsl:value-of select="title" /></title>
               <style media="all" lang="en" type="text/css">
                    .ChannelTitle
                    {
                         font-family:  Verdana;
                         font-size:  11pt;
                         font-weight:  bold;
                         width:  100%;
                         text-align:  center;
                    }
                    .ArticleEntry
                    {
                         border-width:  2px;
                         border-color:  #336699;
                         border-style:  solid;
                         width:  100%;
                    }
                    .ArticleTitle
                    {
                         background-color:  #3366CC;
                         color:  #FFFFFF;
                         font-family:  Verdana;
                         font-size:  9pt;
                         font-weight:  bold;
                         padding-left:  5px;
                         padding-top:  5px;
                         padding-bottom:  5px;
                    }
                    .ArticleHeader
                    {
                         background-color:  #3399FF;
                         color:  #FFFFFF;
                         font-family:  Verdana;
                         font-size:  7pt;
                         padding-left:  5px;
                         padding-top:  2px;
                         padding-bottom:  2px;
                    }
                    .ArticleHeader A:visited
                    {
                         color:  #FFFFFF;
                         text-decoration:  none;
                    }
                    .ArticleHeader A:link
                    {
                         color:  #FFFFFF;
                         text-decoration:  none;
                    }
                    .ArticleHeader A:hover
                    {
                         color:  #FFFF00;
                         text-decoration:  underline;
                    }
                    .ArticleFooter
                    {
                         background-color:  #3399FF;
                         color:  #FFFFFF;
                         font-family:  Verdana;
                         font-size:  7pt;
                         padding-left:  5px;
                         padding-top:  2px;
                         padding-bottom:  2px;
                    }
                    .ArticleFooter A:visited
                    {
                         color:  #FFFFFF;
                         text-decoration:  none;
                    }
                    .ArticleFooter A:link
                    {
                         color:  #FFFFFF;
                         text-decoration:  none;
                    }
                    .ArticleFooter A:hover
                    {
                         color:  #FFFF00;
                         text-decoration:  underline;
                    }
                    .ArticleDescription
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
          <div class="ChannelTitle">
               <xsl:value-of select="text()" />
          </div>
          <br />
     </xsl:template>
     <xsl:template match="item">
          <div class="ArticleEntry">
               <div class="ArticleTitle">
                    <xsl:value-of select="title" />
               </div>
               <div class="ArticleHeader">
                    <a href="{link}">Link</a> - <xsl:value-of select="pubDate" />  - <a href="mailto:{author}">Email The Author</a>
               </div>
               <div class="ArticleDescription">
                    <b>Description:</b> <xsl:value-of select="description" disable-output-escaping="yes"/>
               </div>
               <div class="ArticleFooter">
	               <a href="{comments}">Comments (<xsl:value-of select="slash:comments"/>)</a>
               </div>
          </div>
          <br />
     </xsl:template>
</xsl:stylesheet>