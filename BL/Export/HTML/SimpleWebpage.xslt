<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes" omit-xml-declaration="yes"/>

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <title>myCollections</title>
        <link rel="stylesheet" href="css/lightbox.css" type="text/css" media="screen" />
        <link rel="stylesheet" href="css/style.css" type="text/css" media="screen" />
        <script type="text/javascript" src="js/prototype.lite.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="js/moo.fx.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="js/litebox-1.0.js">
          <xsl:text> </xsl:text>
        </script>
      </head>
      <body onload="initLightbox()">
        <xsl:call-template name="createIndex"/>
        <xsl:for-each select="/myCollections/*">
          <xsl:if test="count(./*) > 0">
            <xsl:call-template name="collection">
              <xsl:with-param name="collection" select="." />
            </xsl:call-template>
          </xsl:if>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>
  <xsl:template name="createIndex">
    <p align="center">
      <xsl:for-each select="/myCollections/*">
        <xsl:if test="count(./*) > 0">
          <a href="#{name(.)}" >
            <xsl:value-of select="name(.)"/>
          </a>
        </xsl:if>
      </xsl:for-each>
    </p>
  </xsl:template>
  <xsl:template name="collection">
    <xsl:param name="collection" />
    <h1>
      <xsl:value-of select="name($collection)"/>
    </h1>
    <a name="{name($collection)}" />
    <p>
      <table>
        <xsl:for-each select="$collection/*">
          <!-- Ugly... but it works! -->
          <xsl:if test="(position() - 1) mod 6 = 0">
            <tr align="center">
              <xsl:call-template name="item-table-cell">
                <xsl:with-param name="item" select="." />
              </xsl:call-template>
              <xsl:if test="following-sibling::node()" >
                <xsl:call-template name="item-table-cell">
                  <xsl:with-param name="item" select="following-sibling::node()" />
                </xsl:call-template>
              </xsl:if>
              <xsl:if test="following-sibling::node()/following-sibling::node()" >
                <xsl:call-template name="item-table-cell">
                  <xsl:with-param name="item" select="following-sibling::node()/following-sibling::node()" />
                </xsl:call-template>
              </xsl:if>
              <xsl:if test="following-sibling::node()/following-sibling::node()/following-sibling::node()" >
                <xsl:call-template name="item-table-cell">
                  <xsl:with-param name="item" select="following-sibling::node()/following-sibling::node()/following-sibling::node()" />
                </xsl:call-template>
              </xsl:if>
              <xsl:if test="following-sibling::node()/following-sibling::node()/following-sibling::node()/following-sibling::node()" >
                <xsl:call-template name="item-table-cell">
                  <xsl:with-param name="item" select="following-sibling::node()/following-sibling::node()/following-sibling::node()/following-sibling::node()" />
                </xsl:call-template>
              </xsl:if>
              <xsl:if test="following-sibling::node()/following-sibling::node()/following-sibling::node()/following-sibling::node()/following-sibling::node()" >
                <xsl:call-template name="item-table-cell">
                  <xsl:with-param name="item" select="following-sibling::node()/following-sibling::node()/following-sibling::node()/following-sibling::node()/following-sibling::node()" />
                </xsl:call-template>
              </xsl:if>
            </tr>
          </xsl:if>
        </xsl:for-each>
      </table>
    </p>
  </xsl:template>
  <xsl:template name="item-table-cell">
    <xsl:param name="item"/>
    <td>
      <table>
        <tr>
          <td>
            <a href="images/{$item/Id}.jpg" rel="lightbox" title="{$item/Title}">
              <img src="images/{$item/Id}.jpg" width="200px" class="bordered"/>
            </a>
          </td>
        </tr>
        <tr>
          <td>
            <img src="images/shelf.jpg" width="200px"/>
          </td>
        </tr>
      </table>
    </td>
  </xsl:template>
</xsl:stylesheet>