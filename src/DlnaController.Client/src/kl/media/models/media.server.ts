export interface DeviceVersion {
    Major: number;
    Minor: number;
}

export interface Size {
    Width: number;
    Height: number;
}

export interface Icon {
    Type: string;
    ColorDepth: string;
    Size: Size;
    Url: string;
}

export interface MediaServer {
    DeviceType: string;
    DeviceVersion: DeviceVersion;
    FriendlyName: string;
    Manufacturer: string;
    UDN: string;
    Address: string;
    Icons: Icon[];
}