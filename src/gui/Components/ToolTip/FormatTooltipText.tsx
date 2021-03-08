import S from "gui/Components/ScreenElements/Table/Table.module.scss";
import * as React from "react";

export function formatTooltipText(content: string | string[] | undefined) {
  if (!content) {
    return "";
  }
  const lines = Array.isArray(content)
    ? content.flatMap((line) => splitToLines(line))
    : splitToLines(content);
  return formatToolTipLines(lines);
}

function splitToLines(value: string) {
  return value.split(/\\r\\n|\\n|<br\/>|<BR\/>/);
}

function formatToolTipLines(content: string[]) {
  const equalLengthLines = content; //.flatMap((line) => line.match(/.{1,72}/g));
  const linesToShow =
    equalLengthLines.length > 10 ? equalLengthLines.slice(0, 9).concat(["..."]) : equalLengthLines;
  console.log("/*/*/*", linesToShow);
  return (
    <div className={S.tooltipContent}>
      {linesToShow.map((line) => (
        <div className={S.toolTipLine}>{line}</div>
      ))}
    </div>
  );
}
