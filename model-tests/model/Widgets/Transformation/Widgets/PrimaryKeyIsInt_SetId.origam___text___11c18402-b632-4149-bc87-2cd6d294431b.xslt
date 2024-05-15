﻿<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
	xmlns:AS="http://schema.advantages.cz/AsapFunctions"
	xmlns:date="http://exslt.org/dates-and-times" exclude-result-prefixes="AS date">

	<xsl:template match="/">
		<ROOT>
			<PrimaryKeyIsInt>
				<xsl:attribute name="Id"><xsl:value-of select="1" /></xsl:attribute>
				<xsl:attribute name="GuidId"><xsl:value-of select="AS:FormatNumber(1, '00000000-0000-0000-0000-000000000000')" /></xsl:attribute>
			</PrimaryKeyIsInt>
		</ROOT>
	</xsl:template>

</xsl:stylesheet>