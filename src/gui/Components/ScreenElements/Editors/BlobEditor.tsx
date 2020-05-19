import React from "react";
import { inject, observer } from "mobx-react";
import { getApi } from "model/selectors/getApi";
import { getDataStructureEntityId } from "model/selectors/DataView/getDataStructureEntityId";
import { getSelectedRowId } from "model/selectors/TablePanelView/getSelectedRowId";
import { getMenuItemId } from "model/selectors/getMenuItemId";
import { getEntity } from "model/selectors/DataView/getEntity";
import { getSessionId } from "model/selectors/getSessionId";
import { IApi } from "model/entities/types/IApi";
import { IProperty } from "model/entities/types/IProperty";
import { observable } from "mobx";
import S from "./BlobEditor.module.scss";
import ImageEditor from "tui-image-editor";
import "tui-image-editor/dist/tui-image-editor.css";

@inject(({ property }: { property: IProperty }, { value }) => {
  return {
    api: getApi(property),
    DataStructureEntityId: getDataStructureEntityId(property),
    Property: property.id,
    RowId: getSelectedRowId(property),
    menuItemId: getMenuItemId(property),
    Entity: getEntity(property),
    SessionFormIdentifier: getSessionId(property),
    parameters: property.parameters,
  };
})
@observer
export class BlobEditor extends React.Component<{
  value: string;
  api?: IApi;
  DataStructureEntityId?: string;
  Property?: string;
  RowId?: string;
  menuItemId?: string;
  Entity?: string;
  SessionFormIdentifier?: string;
  parameters?: any;
}> {
  handleFileChange(event: any) {
    this.fileList = event.target.files;
    this.upload();
  }

  @observable.ref fileList: any = [];

  async download() {
    console.log(this.props.parameters);
    const token = await this.props.api!.getDownloadToken({
      SessionFormIdentifier: this.props.SessionFormIdentifier!,
      MenuId: this.props.menuItemId!,
      DataStructureEntityId: this.props.DataStructureEntityId!,
      Entity: this.props.Entity!,
      RowId: this.props.RowId!,
      Property: this.props.Property!,
      FileName: this.props.value,
      parameters: this.props.parameters,
    });
    this.props.api!.getBlob({ downloadToken: token });
  }

  async upload() {
    if (this.fileList && this.fileList.length > 0) {
      for (let file of this.fileList) {
        console.log(file);
        /*if (file.type.startsWith("image")) {
          this.imageObjectUrl = URL.createObjectURL(file);
          this.displayImageEditor = true;

          return;
        }
        return;*/
        const token = await this.props.api!.getUploadToken({
          SessionFormIdentifier: this.props.SessionFormIdentifier!,
          MenuId: this.props.menuItemId!,
          DataStructureEntityId: this.props.DataStructureEntityId!,
          Entity: this.props.Entity!,
          RowId: this.props.RowId!,
          Property: this.props.Property!,
          FileName: this.props.value,
          parameters: this.props.parameters,
          DateCreated: "2010-01-01",
          DateLastModified: "2010-01-01",
        });

        console.log("Uploading ", file.name, file.size);
        await this.props.api!.putBlob({ uploadToken: token, fileName: file.name, file });
        /*const result = await axios.post(`http://localhost:8910/file-upload/${file.name}`, file, {
          headers: { "content-type": "application/octet-stream" },
          onUploadProgress(event) {
            setProgress(event.loaded / event.total);
            if (lastTime !== undefined) {
              setSpeed((event.loaded - lastSize) / (event.timeStamp - lastTime) * 1000);
              console.log(event.loaded - lastSize, event.timeStamp - lastTime)
            }
            lastTime = event.timeStamp;
            lastSize = event.loaded;
          }
        });
        console.log(result)*/
      }
    }
  }

  @observable displayImageEditor = false;
  imageObjectUrl: any;

  render() {
    return (
      <div className={S.blobEditor}>
        {this.displayImageEditor && <ImageEditorCom imageUrl={this.imageObjectUrl} />}
        <input readOnly={true} className="fileName" value={this.props.value} />
        <div className="controls">
          {this.props.value && (
            <button
              className="btnDownload"
              onClick={() => this.download()}
              title={`Download: ${this.props.value}`}
            >
              <i className="fas fa-download"></i>
            </button>
          )}
          <label className="customBtnChoose" title={"Upload new file."}>
            <input
              className="btnChooseFile"
              name="file"
              type="file"
              multiple={false}
              onChange={(event) => this.handleFileChange(event)}
            />
            <i className="fas fa-upload"></i>
          </label>
          {/*<button className="btnStartUpload" onClick={() => this.upload()}>
          Upload file...
    </button>*/}
        </div>
      </div>
    );
  }
}

class ImageEditorCom extends React.Component<{ imageUrl: any }> {
  refImageEditor = (elm: any) => (this.elmImageEditor = elm);
  elmImageEditor: HTMLDivElement | null = null;

  componentDidMount() {
    var instance = new ImageEditor(this.elmImageEditor as any, {
      //cssMaxWidth: 700,
      //cssMaxHeight: 500,
      selectionStyle: {
        cornerSize: 20,
        rotatingPointOffset: 70,
      },
      includeUI: {
        loadImage: { path: this.props.imageUrl, name: "Loaded image" },
        /*loadImage: {
            path: 'img/sampleImage2.png',
            name: 'SampleImage'
        },*/
        // theme: blackTheme, // or whiteTheme
        initMenu: "filter",
        menuBarPosition: "bottom",
      },
    });
  }

  render() {
    return (
      <div className={S.imageEditor}>
        <div ref={this.refImageEditor} className="imageEditorMountpoint" />
      </div>
    );
  }
}
