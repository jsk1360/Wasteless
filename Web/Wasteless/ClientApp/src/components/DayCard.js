import React, {useState} from "react";
import moment from "moment";
import {MenuPicker} from "./MenuPicker";
import {WasteForm} from "./WasteForm";

export const DayCard = (props) => {
    const {waste, menus} = props;

    const [menu, setMenu] = useState(waste.menu);

    const menuChanged = (menu) => {
        setMenu(menu);
    };

    return <div className="card my-3 border-dark" key={waste.date}>
        <div className="card-header">
            <h5>{moment(waste.date).format("dddd D.M.yyyy")}</h5>
        </div>
        <div className="card-body p-0">
            <div className="d-flex flex-column px-3 my-3">
                <h6 className="card-body-label">
                    Ruokalaji
                </h6>
                <MenuPicker menus={menus} selected={waste.menu} onChange={menuChanged}/>
            </div>
            <hr className="m-0"/>
            <div className="p-3 forecast-part">
                <h4 className="mb-3">Ennuste</h4>
                <div className="d-flex flex-column">
                    <h6 className="card-body-label">
                        Ruokailijamäärä
                    </h6>
                    <span style={{height: '1.5rem'}}>{waste.forecastMealTotal}</span>
                </div>

                <div className="d-flex flex-column mt-2">
                    <h6 className="card-body-label">
                        Kokonaishävikkimäärä (kg)
                    </h6>
                    <span style={{height: '1.5rem'}}>{waste.forecastWasteTotalKg}</span>
                </div>

                <div className="d-flex flex-column mt-2">
                    <h6 className="card-body-label">
                        Linjastohävikkimäärä (kg)
                    </h6>
                    <span style={{height: '1.5rem'}}>{waste.forecastLineWasteKg}</span>
                </div>

                <div className="d-flex flex-column mt-2">
                    <h6 className="card-body-label">
                        Lautashävikkimäärä (kg)
                    </h6>
                    <span style={{height: '1.5rem'}}>{waste.forecastPlateWasteKg}</span>
                </div>

                <div className="d-flex flex-column mt-2">
                    <h6 className="card-body-label">
                        Valmistushävikkimäärä (kg)
                    </h6>
                    <span style={{height: '1.5rem'}}>{waste.forecastProductionWasteKg ? waste.forecastProductionWasteKg : "-"}</span>
                </div>
            </div>
            {/*<div className="border-bottom px-3"> </div>*/}
            <hr className="m-0"/>
            <WasteForm waste={waste} menu={menu}/>
        </div>
    </div>
}