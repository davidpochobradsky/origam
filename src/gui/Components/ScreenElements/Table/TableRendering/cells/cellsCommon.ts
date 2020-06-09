import {
  context2d,
  drawingColumnIndex,
  scRenderCell,
  scrollLeft,
  scrollTop
} from "../renderingValues";
import {
  isCurrentCellFixed,
  currentColumnLeft,
  currentRowTop,
  currentColumnWidth,
  currentRowHeight,
} from "../currentCell";
import { CPR } from "utils/canvas";
import {Memoized} from "../common/Memoized";

export function applyScrollTranslation() {
  const ctx2d = context2d();
  ctx2d.translate(!isCurrentCellFixed() ? -CPR * scrollLeft() : 0, -CPR * scrollTop());
}

export function clipCell() {
  const ctx2d = context2d();
  ctx2d.beginPath();
  ctx2d.rect(
    CPR * currentColumnLeft(),
    CPR * currentRowTop(),
    CPR * currentColumnWidth(),
    CPR * currentRowHeight()
  );
  ctx2d.clip();
}

export const numberCellPaddingLeft = Memoized(() =>  drawingColumnIndex() === 0 ? 25 : 15)
scRenderCell.push(() => numberCellPaddingLeft.clear());

export const cellPaddingLeft = 14;

export const topTextOffset = 17;

export const fontSize = 12;