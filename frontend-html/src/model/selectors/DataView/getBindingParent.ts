import {getBindingParents} from "./getBindingParents";

export function getBindingParent(ctx: any) {
  const bps = getBindingParents(ctx);
  const bp = bps.length > 0 ? bps[0] : undefined;
  return bp;
}