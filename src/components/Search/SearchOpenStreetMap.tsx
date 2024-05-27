import React, { useEffect, useState } from 'react';
import { Feature, Map, View } from 'ol';
import { Tile as TileLayer, Vector as VectorLayer } from 'ol/layer';
import { OSM, Vector as VectorSource } from 'ol/source';
import Point from 'ol/geom/Point';
import { Interaction } from 'ol/interaction';
import 'ol/ol.css';
import '../../assets/Index.css';

const SearchOpenStreetMap: React.FC = () => {
    const [map, setMap] = useState<Map | null>(null);

    useEffect(() => {
        if (!map) {
            const initialCoordinates = [300, 300];
            const newMap = new Map({
                target: 'map',
                layers: [
                    new TileLayer({
                        source: new OSM(),
                    }),
                    new VectorLayer({
                        source: new VectorSource(),
                    }),
                ],
                view: new View({
                    center: initialCoordinates,
                    zoom: 2,
                }),
            });

            setMap(newMap);

            const vectorSource = new VectorSource();
            const vectorLayer = new VectorLayer({
                source: vectorSource,
            });
            newMap.addLayer(vectorLayer);

            const drawInteraction = new Interaction({
                handleEvent: function (event) {
                    if (event.type === 'click') {
                        const coordinates = newMap.getEventCoordinate(event.originalEvent);
                        const point = new Point(coordinates);
                        const feature = new Feature(point);
                        vectorSource.clear();
                        vectorSource.addFeature(feature);
                    }
                    return true;
                },
            });

            newMap.addInteraction(drawInteraction);
        }
    }, [map]);

    return (
        <div id="map" className="map" style={{ width: '300px', height: '150px' ,borderRadius: 0, borderBottomRightRadius: '3px', borderTopRightRadius: '3px'}}></div>
    );
};

export default SearchOpenStreetMap;