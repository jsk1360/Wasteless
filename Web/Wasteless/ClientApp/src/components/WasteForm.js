import React, {useEffect, useReducer, useState} from "react";
import moment from "moment";
import Input from "./Input";
import {useAccessToken} from "../accessTokenContext";

export const WasteForm = (props) => {
    function formReducer(prevState, {value, key}) {
        return {...prevState, [key]: value};
    }

    const [accessToken] = useAccessToken()

    const [formState, dispatch] = useReducer(formReducer, {
        id: "",
        date: "",
        locationId: "",
        mealTotal: "",
        specialMealCount: "",
        mealCountReserved: "",
        plateWasteKg: "",
        productionWasteKg: "",
        menuItemWaste: [],
        menu: "",
        comment: "",
    });

    const [isSaving, setIsSaving] = useState(false);
    const [isInitialized, setIsInitialized] = useState(false);

    useEffect(() => {
        dispatch({key: "id", value: props.waste.id});
        dispatch({key: "date", value: props.waste.date});
        dispatch({
            key: "locationId",
            value: props.waste.locationId
        });
        dispatch({key: "menu", value: props.menu === undefined ? "" : props.menu.name});

        // Ei päivitetä, jos tietoja on muokattu käyttäjän toimesta
        if (isInitialized) return;
        dispatch({
            key: "mealTotal",
            value: props.waste.mealTotal !== null ? props.waste.mealTotal : ""
        });
        dispatch({
            key: "specialMealCount",
            value: props.waste.specialMealCount !== null ? props.waste.specialMealCount : ""
        });
        dispatch({
            key: "mealCountReserved",
            value: props.waste.mealCountReserved !== null ? props.waste.mealCountReserved : ""
        });
        dispatch({
            key: "plateWasteKg",
            value: props.waste.plateWasteKg !== null ? props.waste.plateWasteKg : ""
        });
        dispatch({
            key: "productionWasteKg",
            value: props.waste.productionWasteKg !== null ? props.waste.productionWasteKg : ""
        });
        dispatch({
            key: "menuItemWaste",
            value: props.waste.menuItemWaste !== null ? props.waste.menuItemWaste : []
        });
        dispatch({
            key: "comment",
            value: props.waste.comment !== null ? props.waste.comment : ""
        });
        setIsInitialized(true);
    }, [props.waste, props.menu, isInitialized]);

    const [overLimit] = useState(false);

    useEffect(() => {
        // console.log(formState.lineWasteKg + formState.productionWasteKg + formState.plateWasteKg, props.waste.wasteLimit ,formState.lineWasteKg + formState.productionWasteKg + formState.plateWasteKg > props.waste.wasteLimit);
        // if (formState.lineWasteKg + formState.productionWasteKg + formState.plateWasteKg > props.waste.wasteLimit) { 
        //     setOverLimit(true);
        // } else {
        //     setOverLimit(false);
        // }
    }, [props.waste, formState])


    async function handleSubmit(event) {
        event.preventDefault();
        if (isValid(formState)) {
            setIsSaving(true);
            const formCopy = {...formState};
            formCopy.date = moment(formState.date).format("yyyy-MM-DD");
            formCopy.specialMealCount = formState.specialMealCount !== "" ? formState.specialMealCount : null;
            const response = await fetch('waste', {
                method: 'POST',
                body: JSON.stringify(formCopy),
                headers: !accessToken ? {'Content-Type': 'application/json'} : {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${accessToken}`
                }
            });
            if (response.ok) {
                const data = await response.json();
                dispatch({key: "id", value: data.id});
            } else {
                alert("Tallentaessa tapahtui virhe");
            }

            setIsSaving(false);
        } else {
            console.log("notvalid", formState);
        }
    }

    function isValid(form) {
        const isV = form.date !== "" &&
            form.mealTotal !== "" &&
            form.mealCountReserved !== "" &&
            form.plateWasteKg !== "" &&
            form.menu !== "";

        // if (overLimit) {
        //     return isV && form.comment !== "";
        // }
        return isV;
    }

    function getCommentLabel(overLimit) {
        return overLimit ? "Kommentoi rajan ylittävää hävikkiä" : "Vapaaehtoinen kommentti"
    }

    function updateMenuItemWaste(itemId, value, property) {
        const newMenuItemWaste = [...formState.menuItemWaste];
        const menuItem = newMenuItemWaste.find(miw => miw.item.id === itemId);
        if (menuItem) {
            menuItem[property] = value;
            dispatch({key: "menuItemWaste", value: newMenuItemWaste});
        }
    }

    function sumAllWaste(waste) {
        const num = formState.menuItemWaste.reduce((acc, cur) => acc + cur.lineWasteKg, waste.plateWasteKg + waste.productionWasteKg);
        return round(num);
    }
    
    function round(num) {
        return Math.round(num * 100 + Number.EPSILON) / 100;
    }

    return (
        <form onSubmit={handleSubmit}>
            <div className="p-3 production-part">
                <h4 className="fw-bold">Valmistus</h4>
                <Input
                    changed={({target: {value}}) => dispatch({value: +value, key: "mealCountReserved"})}
                    value={formState["mealCountReserved"]}
                    id={"mealCountReserved" + formState.date}
                    name="mealCountReserved"
                    label="Ruokaa valmistettu ruokailijamäärälle"
                    className="form-control form-control-sm mb-3"
                    type="number"
                />
                <div className="row gap-2 mb-3">
                    {formState.menuItemWaste.map(w =>
                        <div className="col-md d-flex flex-column justify-content-end"
                             key={`producedKg_${formState.date}_${w.item.name}`}>
                            <Input
                                readOnly={true}
                                formGroup={false}
                                changed={({target: {value}}) => updateMenuItemWaste(w.item.id, +value, "producedKg")}
                                value={w.producedKg}
                                id={`producedKg_${formState.date}_${w.item.name}`}
                                name="producedKg"
                                label={w.item.name}
                                className="form-control form-control-sm"
                                type="number"
                            />
                        </div>
                    )}</div>
            </div>
            <hr className="m-0"/>
            <div className="p-3 actual-part">
                <h4 className="fw-bold">Toteuma</h4>
                <Input
                    changed={({target: {value}}) => dispatch({value: +value, key: "mealTotal"})}
                    value={formState["mealTotal"]}
                    id={"mealTotal" + formState.date}
                    name="mealTotal"
                    label="Ruokailijamäärä (kaikki yhteensä)"
                    className="form-control form-control-sm mb-3"
                    type="number"
                />
                <Input
                    changed={({target: {value}}) => dispatch({
                        value: value === '' ? value : +value,
                        key: "specialMealCount"
                    })}
                    value={formState["specialMealCount"]}
                    id={"specialMealCount" + formState.date}
                    name="specialMealCount"
                    label="Erikoisruokavalion ruokailijamäärä eriteltynä"
                    className="form-control form-control-sm mb-3"
                    type="number"
                />
                <hr/>
                <h5 className="mb-3 fw-bold">
                    Hävikkimäärä (kg)
                </h5>
                <Input
                    readOnly={true}
                    value={sumAllWaste(formState)}
                    id={"wasteTotal" + formState.date}
                    name="wasteTotal"
                    label="Päivän kokonaishävikki"
                    className="form-control form-control-sm mb-3"
                    type="number"
                />
                <hr/>
                <h6 className="fw-bold">Linjasto</h6>
                <div className="row gap-2 mb-3">
                    {formState.menuItemWaste.map(w =>
                        <div className="col-md d-flex flex-column justify-content-end"
                             key={`lineWasteKg_${formState.date}_${w.item.name}`}>
                            <Input
                                formGroup={false}
                                changed={({target: {value}}) => updateMenuItemWaste(w.item.id, +value, "lineWasteKg")}
                                value={w.lineWasteKg}
                                id={`lineWasteKg_${formState.date}_${w.item.name}`}
                                name="lineWasteKg"
                                label={w.item.name}
                                className="form-control form-control-sm"
                                type="number"
                            />
                        </div>
                    )}
                </div>
                <div className="d-flex flex-row gap-2 align-items-center">
                    <span className="fw-bolder">Yhteensä:</span>
                    <span>
                        {round(formState.menuItemWaste.reduce((acc, cur) => acc + cur.lineWasteKg, 0))}
                    </span>
                </div>
                <hr/>
                <Input
                    changed={({target: {value}}) => dispatch({value: +value, key: "productionWasteKg"})}
                    value={formState["productionWasteKg"]}
                    id={"productionWasteKg" + formState.date}
                    name="productionWasteKg"
                    readOnly
                    label={<h6 className="fw-bold">Valmistus</h6>}
                    className="form-control form-control-sm mb-3"
                    type="number"
                />
                <hr/>
                <Input
                    changed={({target: {value}}) => dispatch({value: +value, key: "plateWasteKg"})}
                    value={formState["plateWasteKg"]}
                    id={"plateWasteKg" + formState.date}
                    name="plateWasteKg"
                    readOnly
                    label={<h6 className="fw-bold">Lautas</h6>}
                    className="form-control form-control-sm mb-3"
                    type="number"
                />
                <hr/>
                <Input
                    changed={({target: {value}}) => dispatch({value: value, key: "comment"})}
                    value={formState["comment"]}
                    id={"comment" + formState.comment}
                    name="comment"
                    label={getCommentLabel(overLimit)}
                    className="form-control form-control-sm mb-3"
                    type="text"
                />
                <button className="btn btn-primary btn-sm" type="submit" disabled={isSaving || !isValid(formState)}>
                    {isSaving ?
                        <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> :
                        <></>
                    }
                    Tallenna
                </button>
            </div>
        </form>
    )
}