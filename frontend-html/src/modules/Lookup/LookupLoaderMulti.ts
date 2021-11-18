import { action, observable, createAtom } from "mobx";
import { PubSub } from "./common";
import { LookupApi, ILookupApi } from "./LookupApi";
import { Clock, IClock } from "./Clock";
import { TypeSymbol } from "dic/Container";

export interface ILookupMultiResultListenerArgs {
  labels: Map<string, Map<any, any>>;
}

export class LookupLoaderMulti {
  constructor(private clock: Clock, private api: LookupApi) {
    this.triggerLoadDeb = clock.debounce(this.triggerLoadImm, 667);
  }

  // lookupId -> lookupKey -> something 🦄
  interrests = new Map<string, Map<any, any>>();
  loading = new Map<string, Map<any, any>>();
  loadingAtom = createAtom(
    "LookupLoading",
    () => {},
    () => {}
  );

  resultListeners = new PubSub<ILookupMultiResultListenerArgs>();

  @observable isLoading = false;

  @action.bound
  async triggerLoadImm() {
    // TODO: Rewrite as a flow to preserve mobx transaction
    if (this.isLoading) return;
    try {
      do {
        this.isLoading = true;
        /*console.log("Will load:");
        for (let [l1k, l1v] of this.interrests.entries()) {
          console.log(`  ${l1k}:`);
          for (let [l2k, l2v] of l1v.entries()) {
            console.log(`    ${l2k}`);
          }
        }*/

        for (let [k, v] of this.interrests.entries()) {
          this.loading.set(k, v);
        }
        this.interrests.clear();
        this.loadingAtom.reportChanged();

        const result = await this.api.getLookupLabels(this.loading);

        for (let [l1k, l1v] of result.entries()) {
          l1k = String(l1k).toLowerCase();
          if (!this.loading.has(l1k)) continue;
          for (let [l2k, l2v] of l1v.entries()) {
            l2k = String(l2k).toLowerCase();
            this.loading.get(l1k)!.delete(l2k);
          }
          if (this.loading.get(l1k)!.size === 0) {
            this.loading.delete(l1k);
          }
        }
        this.loadingAtom.reportChanged();

        this.resultListeners.trigger({ labels: result });
      } while (this.interrests.size > 0);
    } finally {
      this.isLoading = false;
    }
  }

  triggerLoadDeb = () => {};

  setInterrest(lookupId: string, key: any) {
    // Maybe it is loading right now.
    if (this.loading.has(lookupId) && this.loading.get(lookupId)!.has(key)) return;

    // Not yet loading, record that someon is interrested.
    if (!this.interrests.has(lookupId)) {
      this.interrests.set(lookupId, new Map());
    }
    const lookupInterrests = this.interrests.get(lookupId)!;
    lookupInterrests.set(key, true);

    // Schedule actual loading.
    this.triggerLoadDeb();
  }

  resetInterrest(lookupId: string, key: any) {
    if (!this.interrests.has(lookupId)) {
      return;
    }
    const lookupInterrests = this.interrests.get(lookupId)!;
    lookupInterrests.delete(key);
    if (lookupInterrests.size === 0) {
      this.interrests.delete(lookupId);
    }
  }

  async loadList(lookupId: string, labelIds: Set<any>) {
    return this.api.getLookupLabels(
      new Map([[lookupId, new Map(Array.from(labelIds.keys()).map((labelId) => [labelId, true]))]])
    );
  }

  isWorking(lookupId: string, key: any) {
    this.loadingAtom.reportObserved();
    return this.loading.get(lookupId)?.has(key) || false;
  }
}
export const ILookupLoaderMulti = TypeSymbol<LookupLoaderMulti>("ILookupLoaderMulti");
