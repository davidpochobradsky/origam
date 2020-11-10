import { computed } from "mobx";
import { parse as wktParse } from "wkt";
import { MapRootStore } from "./MapRootStore";

export class MapLayer {
  id: string = "";
  title: string = "";
  defaultEnabled: boolean = false;
  type: string = "";
  mapLayerParameters = new Map<string, any>();

  getUrl() {
    return this.mapLayerParameters.get("url");
  }

  getTitle() {
    return this.title;
  }

  getOptions() {
    const rawOptions = Object.fromEntries(this.mapLayerParameters);
    delete rawOptions.url;
    delete rawOptions.title;
    return {
      ...rawOptions,
      id: this.id,
      minZoom: rawOptions.minZoom !== undefined ? parseInt(rawOptions.minZoom) : undefined,
      maxZoom: rawOptions.maxZoom !== undefined ? parseInt(rawOptions.maxZoom) : undefined,
    };
  }
}

export class MapSetupStore {
  constructor(private rootStore: MapRootStore) {}

  mapLocationMember: string = "";
  mapAzimuthMember: string = "";
  mapColorMember: string = "";
  mapIconMember: string = "";
  mapTextMember: string = "";
  textColorMember: string = "";
  textLocationMember: string = "";
  textRotationMember: string = "";
  mapCenterRaw: string = "";
  isReadOnlyView: boolean = false;

  layers: MapLayer[] = [];

  @computed
  get mapCenter() {
    try {
      return this.mapCenterRaw ? wktParse(this.mapCenterRaw) : undefined;
    } catch (e) {
      console.error(e);
      return;
    }
  }
}
