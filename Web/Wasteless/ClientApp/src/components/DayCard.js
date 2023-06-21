import React, {useState} from "react";
import moment from "moment";
import {WasteForm} from "./WasteForm";

export const NumberFormat = (props) => {
    function formatNumber(num) {
        return Math.round(num * 100 + Number.EPSILON) / 100;
    }

    return (
        <>
            {formatNumber(props.value)}
        </>
    )
}

export const DayCard = (props) => {
    const {waste} = props;

    const [menu] = useState(waste.menu);
    const [collapsed, setCollapsed] = useState(!isToday(waste.date));
    
    function isToday(date) {
        return moment().isSame(moment(date), 'day');
    }

    function printTotal(total) {
        if (total && total !== 0) {
            return <NumberFormat value={total}/>;
        }

        return "-";
    }

    function calculateTotal(numbers) {
        const total = numbers.reduce((a, b) => a + b);
        return printTotal(total);
    }

    return <div className="card my-3 border-dark" key={waste.date}>
        <div className="card-header">
            <div className="d-flex flex-row w-100 align-items-center" style={{cursor: 'pointer'}}
                 onClick={() => setCollapsed(!collapsed)}>
                <h5 className="m-0">{moment(waste.date).format("dddd D.M.yyyy")}</h5>
                <div className="ms-auto">
                    {collapsed ?
                        <i className="bi bi-arrows-expand fs-5"/> :
                        <i className="bi bi-arrows-collapse fs-5"/>
                    }
                </div>
            </div>
        </div>
        {!collapsed &&
            <div className="card-body p-0">
                <div className="d-flex flex-column px-3 my-3">
                    <h4 className="card-body-label fw-bold">
                        Ruokalaji
                    </h4>
                    <p className="m-0">{waste.menu}</p>
                </div>
                <hr className="m-0"/>
                <div className="p-3 forecast-part">
                    <h4 className="mb-3 fw-bold">Ennuste</h4>
                    <div className="d-flex flex-column">
                        <h6 className="card-body-label">
                            Ruokailijamäärä
                        </h6>
                        <span style={{height: '1.5rem'}}>{waste.forecastMealTotal ?? "-"}</span>
                    </div>

                    <div className="d-flex flex-column mt-2">
                        <div className="table-responsive">
                            <table className="table caption-top">
                                <caption>
                                    Hävikkimäärät kiloissa
                                </caption>
                                <thead>
                                <tr>
                                    <th>Ruokalaji</th>
                                    <th>Linjasto</th>
                                    <th>Valmistus</th>
                                    <th>Lautas</th>
                                    <th>Kokonais</th>
                                </tr>
                                </thead>
                                <tbody>
                                {waste.forecastMenuItemWaste.map(w =>
                                    <tr key={w.item.name}>
                                        <td>{w.item.name}</td>
                                        <td>{w.lineWasteKg ?? "-"}</td>
                                        <td>{w.productionWasteKg ?? "-"}</td>
                                        <td>{w.plateWasteKg ?? "-"}</td>
                                        <td>{w.totalWasteKg ?? "-"}</td>
                                    </tr>
                                )}
                                </tbody>
                                <tfoot>
                                <tr>
                                    <td>Yhteensä</td>
                                    <td>{calculateTotal(waste.forecastMenuItemWaste.map(w => w.lineWasteKg))}</td>
                                    <td>{calculateTotal(waste.forecastMenuItemWaste.map(w => w.productionWasteKg))}</td>
                                    <td>{calculateTotal(waste.forecastMenuItemWaste.map(w => w.plateWasteKg))}</td>
                                    <td>{calculateTotal(waste.forecastMenuItemWaste.map(w => w.totalWasteKg))}</td>
                                </tr>
                                </tfoot>
                            </table>
                        </div>
                    </div>

                </div>
                <hr className="m-0"/>
                <WasteForm waste={waste} menu={menu}/>
            </div>}
    </div>
}