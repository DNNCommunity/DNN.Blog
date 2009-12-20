<?xml version="1.0" ?> 
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"> 
<xsl:output method="xml" encoding="iso-8859-1" omit-xml-declaration="yes" indent="yes"/> 
<xsl:template match="*"> 
    <table border="1" width="100%" align="center"> 
        <tr>
            <td valign="top" align="center" class="subhead" bgcolor="silver" > 
                <a> <xsl:attribute name="href"> <xsl:value-of select="*[local-name()='channel']/*[local-name()='link']"/> 
                    </xsl:attribute> <xsl:attribute name="target"> <xsl:text>top</xsl:text> </xsl:attribute> 
                    <xsl:value-of select="*[local-name()='channel']/*[local-name()='title']" disable-output-escaping="yes"/> 
                </a> 
            </td>
        </tr>
        <tr>
        <td class="normal" align="center">
                <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text> 
                <xsl:value-of select="*[local-name()='channel']/*[local-name()='pubDate']"/>             
        </td>
        </tr>
        <tr>
            <td valign="top" class="normal">
                    <xsl:for-each select="//*[local-name()='item']"> 
                        <a class="subhead"> <xsl:attribute name="href"> <xsl:value-of select="*[local-name()='link']"/> 
                            </xsl:attribute> <xsl:attribute name="target"> <xsl:text>_self</xsl:text> </xsl:attribute>
                            <xsl:value-of select="*[local-name()='title']" disable-output-escaping="yes"/> 
                        </a>
                        <br></br>
                    </xsl:for-each> 
            </td>
        </tr> 
    </table> 
    </xsl:template> 
    <xsl:template match="/"> 
    <xsl:apply-templates/> 
    </xsl:template> 
</xsl:stylesheet>