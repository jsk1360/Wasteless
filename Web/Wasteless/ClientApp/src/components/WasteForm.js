import React, {useEffect, useReducer, useState} from "react";
import moment from "moment";
import authService from "./api-authorization/AuthorizeService";
import Input from "./Input";

export const WasteForm = (props) => {
    function formReducer(prevState, {value, key}) {
        return {...prevState, [key]: value};
    }

    const [formState, dispatch] = useReducer(formReducer, {
        id: "",
        date: "",
        locationId: "",
        mealTotal: "",
        specialMealCount: "",
        mealCountReserved: "",
        lineWasteKg: "",
        plateWasteKg: "",
        productionWasteKg: "",
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
            key: "lineWasteKg",
            value: props.waste.lineWasteKg !== null ? props.waste.lineWasteKg : ""
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
            key: "comment",
            value: props.waste.comment !== null ? props.waste.comment : ""
        });
        setIsInitialized(true);
    }, [props.waste, props.menu, isInitialized]);
    
    const [overLimit, setOverLimit] = useState(false);
    
    useEffect(() => {
        console.log(formState.lineWasteKg + formState.productionWasteKg + formState.plateWasteKg, props.waste.wasteLimit ,formState.lineWasteKg + formState.productionWasteKg + formState.plateWasteKg > props.waste.wasteLimit);
        if (formState.lineWasteKg + formState.productionWasteKg + formState.plateWasteKg > props.waste.wasteLimit) { 
            setOverLimit(true);
        } else {
            setOverLimit(false);
        }
    }, [props.waste, formState])


    async function handleSubmit(event) {
        event.preventDefault();
        if (isValid(formState)) {
            setIsSaving(true);
            const formCopy = {...formState};
            formCopy.date = moment(formState.date).format("yyyy-MM-DD");
            formCopy.specialMealCount = formState.specialMealCount !== "" ? formState.specialMealCount : null;
            var token = await authService.getAccessToken();
            const response = await fetch('waste', {
                method: 'POST',
                body: JSON.stringify(formCopy),
                headers: !token ? {'Content-Type': 'application/json'} : {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
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
            form.lineWasteKg !== "" &&
            form.plateWasteKg !== "" &&
            form.productionWasteKg !== "" &&
            form.menu !== "";

        // if (overLimit) {
        //     return isV && form.comment !== "";
        // }
        return isV;
    }
    
    function getCommentLabel(overLimit) {
        return overLimit ? "Kommentoi rajan ylittävää hävikkiä" : "Vapaaehtoinen kommentti"
    }

    return (
        <form onSubmit={handleSubmit}>
            <div className="p-3 production-part">
                <h4>Valmistus</h4>
                <Input
                    changed={({target: {value}}) => dispatch({value: +value, key: "mealCountReserved"})}
                    value={formState["mealCountReserved"]}
                    id={"mealCountReserved" + formState.date}
                    name="mealCountReserved"
                    label="Ruokaa valmistettu ruokailijamäärälle"
                    className="form-control form-control-sm"
                    type="number"
                />
            </div>
            <hr className="m-0"/>
            <div className="p-3 actual-part">
                <h4>Toteuma</h4>
                <Input
                    changed={({target: {value}}) => dispatch({value: +value, key: "mealTotal"})}
                    value={formState["mealTotal"]}
                    id={"mealTotal" + formState.date}
                    name="mealTotal"
                    label="Ruokailijamäärä (kaikki yhteensä)"
                    className="form-control form-control-sm"
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
                    className="form-control form-control-sm"
                    type="number"
                />
                <h6>
                    Hävikkimäärä (kg)
                </h6>
                <Input
                    changed={({target: {value}}) => dispatch({value: +value, key: "lineWasteKg"})}
                    value={formState["lineWasteKg"]}
                    id={"lineWasteKg" + formState.date}
                    name="lineWasteKg"
                    label="Linjasto"
                    className="form-control form-control-sm"
                    type="number"
                />
                <Input
                    changed={({target: {value}}) => dispatch({value: +value, key: "productionWasteKg"})}
                    value={formState["productionWasteKg"]}
                    id={"productionWasteKg" + formState.date}
                    name="productionWasteKg"
                    label="Valmistus"
                    className="form-control form-control-sm"
                    type="number"
                />
                <Input
                    changed={({target: {value}}) => dispatch({value: +value, key: "plateWasteKg"})}
                    value={formState["plateWasteKg"]}
                    id={"plateWasteKg" + formState.date}
                    name="plateWasteKg"
                    label="Lautas"
                    className="form-control form-control-sm"
                    type="number"
                />
                <>
                    <Input
                        changed={({target: {value}}) => dispatch({value: value, key: "comment"})}
                        value={formState["comment"]}
                        id={"comment" + formState.comment}
                        name="comment"
                        label={getCommentLabel(overLimit)}
                        className="form-control form-control-sm"
                        type="text"
                    />
                </>
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